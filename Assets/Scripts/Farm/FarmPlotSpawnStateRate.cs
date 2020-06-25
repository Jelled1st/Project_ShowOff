using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FarmPlotSpawnStateRate", menuName = "ScriptableObjects/FarmPlotSpawnStateRate", order = 1)]
public class FarmPlotSpawnStateRate : ScriptableObject, IComparable
{
    [Header("Open Script for help")]
    /*
     * This scriptable object is used in the farm game handler
     * and is used for setting the spawn rate of plot states at the
     * start of the game.
     * 
     * Variables
     * -----------------------------------
     * int importance:
     * The StateSpawnRate with the highest importance will be
     * computed first. If for example there are two states with
     * a no max, 100% spawn rate the one computed first will
     * take up all the plots and all the others are discarded
     * 
     * FarmPlot.State state:
     * Sets the state for the spawn rate
     *      
     * int spawnRate:
     * The chance (in percentage) that a plot spawns with the
     * set state
     * 
     * uint (unsigned int) minSpawns:
     * the minimal amount of plots that should start with the
     * set state
     * 
     * int maxSpawns:
     * The max amount of plots that can start with the set state
     * Set value -1 for no max
     * if min is higher than max, the max value will be ignored
     * 
     * int finalPlotAmount:
     * If you do not want a certain chance for spawning with
     * a state but rather a set amount of plots you can set this
     * Set value to -1 to not use this, if set to any other value
     * the other values will be ignored
     * 
     */
    public int importance;

    public FarmPlot.State state;

    [Range(0, 100)]
    public int spawnRate = 10;

    public uint minSpawns = 0;
    public int maxSpawns = 2;
    public int finalPlotAmount = -1;

    public int CompareTo(object obj)
    {
        if (obj is FarmPlotSpawnStateRate)
        {
            var other = (FarmPlotSpawnStateRate) obj;
            return other.importance - importance;
        }
        else
        {
            return 0;
        }
    }
}