using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterMortarManning;

public static class RectUtility
{
    public static CellRect FromTo(IntVec3 from, IntVec3 to)
    {
        if (from.x > to.x)
            (from.x, to.x) = (to.x, from.x);
        
        if (from.z > to.z)
            (from.z, to.z) = (to.z, from.z);
        
        return new(from.x, from.z, to.x - from.x + 1, to.z - from.z + 1);
    }

    // Maybe consider target range for each cell
    public static IEnumerable<IntVec3> GetGridCells(this CellRect rect, int count)
    {
        // If the rect is smaller than the count, just return the cells
        var cells = rect.Cells.ToList();
        if (cells.Count <= count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return cells[i % cells.Count];
            }

            yield break;
        }
        
        
        // Calculate the rectangle bounds
        int minX = rect.minX;
        int maxX = rect.maxX;
        int minZ = rect.minZ;
        int maxZ = rect.maxZ;
        int width = rect.Width;
        int height = rect.Height;

        // Determine the number of rows and columns in the grid
        int columns = Mathf.CeilToInt(Mathf.Sqrt(count * (width / (float)height)));
        int rows = Mathf.CeilToInt(count / (float)columns);

        // Calculate the step size for rows and columns
        float xStep = width / (float)columns;
        float zStep = height / (float)rows;

        // Generate the target cells
        for (int i = 0; i < count; i++)
        {
            int col = i % columns;
            int row = i / columns;

            // Calculate the cell position
            int x = minX + (int)(col * xStep + xStep / 2);
            int z = minZ + (int)(row * zStep + zStep / 2);

            // Ensure the cell is within the bounds
            x = Mathf.Clamp(x, minX, maxX);
            z = Mathf.Clamp(z, minZ, maxZ);

            yield return new(x, 0, z);
        }
    }
}