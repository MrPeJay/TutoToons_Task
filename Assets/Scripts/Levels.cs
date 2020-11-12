using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class Levels
{
    public List<LevelData> levels;
}

[Serializable]
public class LevelData
{
    public List<string> level_data;

    /// <summary>
    /// Returns all valid coordinates in level data.
    /// </summary>
    /// <returns></returns>
    public List<Vector2> GetCoordinates()
    {
        var coordinateList = new List<Vector2>();

        var coordinateCount = level_data.Count / 2;

        var indexCheck = 0;
        float? prevCoordinate = null;

        for (var i = 0; i < coordinateCount * 2; i++)
        {
            //Check for x coordinate.
            if (indexCheck == 0)
            {
                if (float.TryParse(level_data[i], NumberStyles.Any,
                    CultureInfo.InvariantCulture.NumberFormat, out var result))
                    prevCoordinate = result;

                //If can't parse, set prev coordinate to null, indicating that coordinate is faulty.
                else
                    prevCoordinate = null;

                indexCheck++;
            }
            //Check for y coordinate.
            else
            {
                //Check if coordinate isn't faulty.
                if (float.TryParse(level_data[i], NumberStyles.Any,
                    CultureInfo.InvariantCulture.NumberFormat, out var result))
                {
                    //If x coordinate is not faulty, then add coordinates to the list.
                    if (prevCoordinate != null)
                    {
                        coordinateList.Add(new Vector2(prevCoordinate.Value, -result));
                        //Reset prev coordinate value.
                        prevCoordinate = null;
                    }
                }

                indexCheck--;
            }
        }

        return coordinateList;
    }
}