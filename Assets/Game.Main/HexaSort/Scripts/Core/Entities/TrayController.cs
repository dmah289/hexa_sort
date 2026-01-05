using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HexaSort.Controllers.DifficultyAlgorithm;
using HexaSort.Core.Entities.Grid.Piece;
using HexaSort.Managers.Level;
using manhnd_sdk.Scripts.ConstantKeyNamespace;
using manhnd_sdk.Scripts.ExtensionMethods;
using manhnd_sdk.Scripts.Optimization.PoolingSystem;
using manhnd_sdk.Scripts.SystemDesign;
using manhnd_sdk.Scripts.SystemDesign.EventBus;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HexaSort.Core.Entities.Grid
{
    public class TrayController : MonoSingleton<TrayController>, IEventBusListener
    {
        [Header("Self Components")]
        [SerializeField] private Transform selfTransform;
        
        [Header("Self references")]
        [SerializeField] private Transform[] hexStackHolders;
        [SerializeField] private HexStackController[] hexStacks;
        
        [Header("Configurations")]
        [SerializeField] private Vector2 spawnMidStackPos;
        [SerializeField] private int remainStackAmount;

        [Header("----- Spawn Configurations -----")]
        [SerializeField] private List<ColorType> rescuedColorsThisTurn;
        [SerializeField] private bool minAmountColorPicked;
        [SerializeField] private int remainingMercySpawnTurn;
        
        [Header("Self references")]
        [SerializeField] private GridController grid;

        #region Unity Callbacks
        
        protected override void Awake()
        {
            base.Awake();
            selfTransform = transform;
            
            // Initialize hexStacks array with size 3
            if (hexStacks == null || hexStacks.Length != 3)
            {
                hexStacks = new HexStackController[3];
            }
            
            RegisterCallbacks();
        }

        #endregion

        #region API

        public void SetupTray(float centerPosX, float minCellY, float maxCellX, int height)
        {
            gameObject.SetActive(true);
            
            // Ensure hexStacks array is properly initialized
            if (hexStacks == null || hexStacks.Length != 3)
            {
                hexStacks = new HexStackController[3];
            }
            
            float trayToGridOffsetY = 0.157f * height + 0.314f;
            
            Vector2 targetPos= new Vector2(
                centerPosX,
                minCellY - ConstantKey.BOARD_CELL_r - trayToGridOffsetY);
            
            spawnMidStackPos = new Vector2(
                maxCellX + ConstantKey.BOARD_CELL_R + ConstantKey.SPAWN_POS_OFFSET_X + ConstantKey.TRAY_WIDTH / 2f,
                targetPos.y);
            
            selfTransform.position = targetPos;
            remainingMercySpawnTurn = ConstantKey.MaxMercySpawnTurnPerLevel;
            
            SpawnHexStacks().Forget();
        }
        
        public void CleanUpTray()
        {
            CleanUpCurrentStacks();
            gameObject.SetActive(false);
        }
        
        public void CleanUpCurrentStacks()
        {
            for (int i = 0; i < hexStacks.Length; i++)
            {
                if (hexStacks[i])
                {
                    ObjectPooler.ReturnToPool(PoolingType.HexStack, hexStacks[i], destroyCancellationToken);
                    hexStacks[i] = null;
                }
            }
        }

        #endregion

        #region Class Methods

        private async UniTask SpawnHexStacks(bool allowWaitingSliding = false)
        {
            GameDifficultyController.Instance.SpawnCycleCounter++;

            // 5 : if 6 empty -> spawn new 3 stacks
            // -> after that remains 3 empty cells respectively 3 new spawned stacks
            if (grid.EmptyCells.Count <= 5 && remainingMercySpawnTurn > 0)
            {
                remainingMercySpawnTurn--;
                await SpawnMercyStacks(allowWaitingSliding);
            }
            else
            {
                if (GameDifficultyController.Instance.IsRandomSpawnTurn)
                    await SpawnStacksRandomly(allowWaitingSliding);
                else
                    await SpawnStacksToRescue(allowWaitingSliding);
            }

            await UniTask.DelayFrame(5);

            remainStackAmount = 3;
        }

        private async UniTask SpawnStacksRandomly(bool allowWaitingSliding)
        {
            List<ColorType> spawnableColors = LevelManager.Instance.SpawnableColors;
            
            for (int i = 0; i < 3; i++)
            {
                int colorAmount = Random.Range(1, GameDifficultyController.Instance.MaxColorPerStack+1);
                List<ColorType> chosenColors = spawnableColors.GetRandomElements(colorAmount);
                
                hexStacks[i] = await ObjectPooler.GetFromPool<HexStackController>(
                    PoolingType.HexStack, destroyCancellationToken, hexStackHolders[i]);

                if (allowWaitingSliding)
                {
                    await hexStacks[i].OnSpawningOnTray(i, 
                        spawnMidStackPos, chosenColors, true);
                }
                else
                {
                    hexStacks[i].OnSpawningOnTray(i, spawnMidStackPos, chosenColors).Forget();
                }
            }
        }
        
        private async UniTask SpawnStacksToRescue(bool allowWaitingSliding)
        {
            rescuedColorsThisTurn = GameDifficultyController.Instance.GetRescuedColor();
            ColorType minAmountColor = rescuedColorsThisTurn[^1];
            minAmountColorPicked = false;
            
            for (int i = 0; i < 3; i++)
            {
                int colorAmount = Random.Range(1, GameDifficultyController.Instance.MaxColorPerStack+1);
                List<ColorType> chosenColors = rescuedColorsThisTurn.GetRandomElementsPreserveOrder(colorAmount);

                if (chosenColors.Contains(minAmountColor))
                {
                    Debug.Log("Picked min amount color");
                    minAmountColorPicked = true;
                }

                if (i == 2 && !minAmountColorPicked)
                {
                    Debug.Log("Force add min amount color");
                    chosenColors.RemoveFirst();
                    chosenColors.Add(minAmountColor);
                }
                
                hexStacks[i] = await ObjectPooler.GetFromPool<HexStackController>(
                    PoolingType.HexStack, destroyCancellationToken, hexStackHolders[i]);
        
                if (allowWaitingSliding)
                {
                    await hexStacks[i].OnSpawningOnTray(i, 
                        spawnMidStackPos, chosenColors, true);
                }
                else 
                {
                    hexStacks[i].OnSpawningOnTray(i, spawnMidStackPos, chosenColors).Forget();
                }
            }
        }

        private async UniTask SpawnMercyStacks(bool allowWaitingSliding = false)
        {
            Debug.Log("Spawning MercyStacks");
            List<HexCell> emptyCells = grid.EmptyCells;
            
            HashSet<HexCell> rescuedCells = new HashSet<HexCell>();
            int stackIndex = 0;

            for (int i = 0; i < emptyCells.Count && stackIndex < 3; i++)
            {
                HexCell emptyCell = emptyCells[i];
                List<HexCell> neighbors = grid.GetNeighbors(emptyCell);
                
                // Filter out empty neighbors, only keep occupied ones
                List<HexCell> neighborsHasStack = new List<HexCell>();
                for (int j = 0; j < neighbors.Count; j++)
                {
                    if (neighbors[j] != null && neighbors[j].CurrentStack != null)
                    {
                        neighborsHasStack.Add(neighbors[j]);
                    }
                }

                if (neighborsHasStack.Count == 0) 
                    continue;

                ColorType? spawnColor = null;
                int spawnAmount = 0;

                // Try to find 2 neighbors with same top color
                for (int j = 0; j < neighborsHasStack.Count - 1; j++)
                {
                    for (int k = j + 1; k < neighborsHasStack.Count; k++)
                    {
                        if (neighborsHasStack[j].ColorOnTop == neighborsHasStack[k].ColorOnTop
                            && !rescuedCells.Contains(neighborsHasStack[j])
                            && !rescuedCells.Contains(neighborsHasStack[k]))
                        {
                            spawnColor = neighborsHasStack[j].ColorOnTop;
                            int totalTopAmount = neighborsHasStack[j].CurrentStack.TopColorAmount 
                                               + neighborsHasStack[k].CurrentStack.TopColorAmount;
                            spawnAmount = Mathf.Max(5, 11 - totalTopAmount);
                            
                            rescuedCells.Add(neighborsHasStack[j]);
                            rescuedCells.Add(neighborsHasStack[k]);
                            break;
                        }
                    }
                    if (spawnColor.HasValue) break;
                }

                // If no matching colors, pick from unrescued neighbors
                if (!spawnColor.HasValue)
                {
                    List<HexCell> unrescuedNeighbors = new List<HexCell>();
                    for (int j = 0; j < neighborsHasStack.Count; j++)
                    {
                        if (!rescuedCells.Contains(neighborsHasStack[j]))
                        {
                            unrescuedNeighbors.Add(neighborsHasStack[j]);
                        }
                    }

                    if (unrescuedNeighbors.Count > 0)
                    {
                        // Pick random colors from up to 2 unrescued neighbors
                        int colorCount = Mathf.Min(2, Mathf.Min(unrescuedNeighbors.Count, GameDifficultyController.Instance.MaxColorPerStack));
                        List<ColorType> chosenColors = new List<ColorType>();
                        List<int> chosenColorAmounts = new List<int>();
                        
                        List<int> pickedIndices = new List<int>();
                        for (int j = 0; j < colorCount; j++)
                        {
                            int randomIdx = Random.Range(0, unrescuedNeighbors.Count);
                            while (pickedIndices.Contains(randomIdx) && pickedIndices.Count < unrescuedNeighbors.Count)
                            {
                                randomIdx = (randomIdx + 1) % unrescuedNeighbors.Count;
                            }
                            
                            if (!pickedIndices.Contains(randomIdx))
                            {
                                pickedIndices.Add(randomIdx);
                                HexCell chosenNeighbor = unrescuedNeighbors[randomIdx];
                                
                                chosenColors.Add(chosenNeighbor.ColorOnTop);
                                int amountToSpawn = Mathf.Max(1, 11 - chosenNeighbor.CurrentStack.TopColorAmount);
                                chosenColorAmounts.Add(amountToSpawn);
                                
                                rescuedCells.Add(chosenNeighbor);
                            }
                        }

                        if (chosenColors.Count > 0)
                        {
                            hexStacks[stackIndex] = await ObjectPooler.GetFromPool<HexStackController>(
                                PoolingType.HexStack, destroyCancellationToken, hexStackHolders[stackIndex]);

                            int[] colorDistribution = chosenColorAmounts.ToArray();

                            if (allowWaitingSliding)
                                await hexStacks[stackIndex].OnSpawningOnTray2(stackIndex, spawnMidStackPos, chosenColors, colorDistribution, true);
                            else
                                hexStacks[stackIndex].OnSpawningOnTray2(stackIndex, spawnMidStackPos, chosenColors, colorDistribution).Forget();

                            stackIndex++;
                            continue;
                        }
                    }

                    // Fallback: pick from rescued neighbors
                    int randomIdx1 = Random.Range(0, neighborsHasStack.Count);
                    spawnColor = neighborsHasStack[randomIdx1].ColorOnTop;
                    spawnAmount = Mathf.Max(1, 11 - neighborsHasStack[randomIdx1].CurrentStack.TopColorAmount);
                }

                // Spawn stack with determined color and amount
                if (spawnColor.HasValue)
                {
                    List<ColorType> chosenColors = new List<ColorType>() {spawnColor.Value};
                    int[] colorDistribution = {spawnAmount};

                    hexStacks[stackIndex] = await ObjectPooler.GetFromPool<HexStackController>(
                        PoolingType.HexStack, destroyCancellationToken, hexStackHolders[stackIndex]);

                    if (allowWaitingSliding)
                        await hexStacks[stackIndex].OnSpawningOnTray2(stackIndex, spawnMidStackPos, chosenColors, colorDistribution, true);
                    else
                        hexStacks[stackIndex].OnSpawningOnTray2(stackIndex, spawnMidStackPos, chosenColors, colorDistribution).Forget();

                    stackIndex++;
                }
            }

            // Fill remaining slots using rescue spawn logic
            if (stackIndex < 3)
            {
                rescuedColorsThisTurn = GameDifficultyController.Instance.GetRescuedColor();
                ColorType minAmountColor = rescuedColorsThisTurn[^1];
                minAmountColorPicked = false;

                for (int i = stackIndex; i < 3; i++)
                {
                    int colorAmount = Random.Range(1, GameDifficultyController.Instance.MaxColorPerStack + 1);
                    List<ColorType> chosenColors = rescuedColorsThisTurn.GetRandomElementsPreserveOrder(colorAmount);

                    if (chosenColors.Contains(minAmountColor))
                    {
                        Debug.Log("Picked min amount color");
                        minAmountColorPicked = true;
                    }

                    if (i == 2 && !minAmountColorPicked)
                    {
                        Debug.Log("Force add min amount color");
                        chosenColors.RemoveFirst();
                        chosenColors.Add(minAmountColor);
                    }

                    hexStacks[i] = await ObjectPooler.GetFromPool<HexStackController>(
                        PoolingType.HexStack, destroyCancellationToken, hexStackHolders[i]);

                    if (allowWaitingSliding)
                        await hexStacks[i].OnSpawningOnTray(i, spawnMidStackPos, chosenColors, true);
                    else
                        hexStacks[i].OnSpawningOnTray(i, spawnMidStackPos, chosenColors).Forget();
                }
            }
        }

        #endregion

        #region Stack Laid Down Listeners

        public void RegisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Register(onEventWithArgs: OnStackLaidDown);
        }

        private void OnStackLaidDown(LaidDownStackDTO data)
        {
            hexStacks[data.cell.CurrentStack.IdxOnTray] = null;
            data.cell.CurrentStack.IdxOnTray = -1;
            
            remainStackAmount--;
            
            if (remainStackAmount == 0)
                SpawnHexStacks().Forget();
        }

        public void DeregisterCallbacks()
        {
            EventBus<LaidDownStackDTO>.Deregister(onEventWithArgs: OnStackLaidDown);
        }

        #endregion

        public async UniTask RespawnCurrentStacks()
        {
            CleanUpCurrentStacks();
            await SpawnHexStacks();
        }
    }
}
