using System.Collections.Generic;
using UnityEngine;

public class City
{
    private const float DefaultBlockLength = 6f;
    private const float BlockLengthVariance = 6f;
    private const float MinimumBlockLength = 2f;

    private readonly List<Intersection> _intersections = new();
    private readonly List<Road> _roads = new();

    public IReadOnlyList<Intersection> Intersections => _intersections;
    public IReadOnlyList<Road> Roads => _roads;

    public static City Generate(int seed, int intersectionCount = 16, int extraConnections = 8)
    {
        var city = new City();

        if (intersectionCount <= 0)
        {
            return city;
        }

        var rng = new System.Random(seed);
        var slotCount = intersectionCount + Mathf.Max(0, extraConnections);
        var columns = Mathf.CeilToInt(Mathf.Sqrt(slotCount));
        var rows = Mathf.CeilToInt((float)slotCount / columns);
        var columnWidths = BuildAxisSpacings(columns, rng);
        var rowHeights = BuildAxisSpacings(rows, rng);
        var xPositions = BuildAxisPositions(columnWidths);
        var yPositions = BuildAxisPositions(rowHeights);
        var occupiedSlots = BuildConnectedOccupiedSlots(rng, columns, rows, intersectionCount);
        var slotToIntersectionID = new Dictionary<int, int>(intersectionCount);

        for (var slotID = 0; slotID < occupiedSlots.Count; slotID++)
        {
            if (!occupiedSlots[slotID])
            {
                continue;
            }

            var column = slotID % columns;
            var row = slotID / columns;
            var position = new Vector2(xPositions[column], yPositions[row]);
            city.AddIntersection(position);

            slotToIntersectionID.Add(slotID, city.Intersections.Count - 1);
        }

        for (var slotID = 0; slotID < occupiedSlots.Count; slotID++)
        {
            if (!occupiedSlots[slotID])
            {
                continue;
            }

            var column = slotID % columns;
            var row = slotID / columns;
            var fromID = slotToIntersectionID[slotID];
            var rightNeighborID = FindNextOccupiedSlotInRow(occupiedSlots, columns, row, column);
            var downNeighborID = FindNextOccupiedSlotInColumn(occupiedSlots, columns, rows, row, column);

            if (rightNeighborID >= 0)
            {
                city.AddBidirectionalRoad(fromID, slotToIntersectionID[rightNeighborID]);
            }

            if (downNeighborID >= 0)
            {
                city.AddBidirectionalRoad(fromID, slotToIntersectionID[downNeighborID]);
            }
        }

        return city;
    }

    private static List<float> BuildAxisSpacings(int count, System.Random rng)
    {
        var spacings = new List<float>(Mathf.Max(0, count - 1));

        for (var i = 0; i < count - 1; i++)
        {
            var variance = ((float)rng.NextDouble() * 2f - 1f) * BlockLengthVariance;
            spacings.Add(Mathf.Max(MinimumBlockLength, DefaultBlockLength + variance));
        }

        return spacings;
    }

    private static List<float> BuildAxisPositions(List<float> spacings)
    {
        var positions = new List<float>(spacings.Count + 1) { 0f };
        var accumulated = 0f;

        foreach (var spacing in spacings)
        {
            accumulated += spacing;
            positions.Add(accumulated);
        }

        var offset = accumulated * 0.5f;

        for (var i = 0; i < positions.Count; i++)
        {
            positions[i] -= offset;
        }

        return positions;
    }

    private static List<bool> BuildConnectedOccupiedSlots(System.Random rng, int columns, int rows, int intersectionCount)
    {
        var occupied = new List<bool>(columns * rows);

        for (var i = 0; i < columns * rows; i++)
        {
            occupied.Add(false);
        }

        var startSlot = rng.Next(occupied.Count);
        occupied[startSlot] = true;

        var frontier = new List<int>();
        var frontierSet = new HashSet<int>();
        AddFrontierNeighbors(startSlot, columns, rows, occupied, frontier, frontierSet);

        var occupiedCount = 1;

        while (occupiedCount < intersectionCount)
        {
            if (frontier.Count == 0)
            {
                break;
            }

            var frontierIndex = rng.Next(frontier.Count);
            var slotID = frontier[frontierIndex];
            frontier.RemoveAt(frontierIndex);
            frontierSet.Remove(slotID);

            if (occupied[slotID])
            {
                continue;
            }

            occupied[slotID] = true;
            occupiedCount++;
            AddFrontierNeighbors(slotID, columns, rows, occupied, frontier, frontierSet);
        }

        return occupied;
    }

    private static void AddFrontierNeighbors(
        int slotID,
        int columns,
        int rows,
        List<bool> occupied,
        List<int> frontier,
        HashSet<int> frontierSet)
    {
        var column = slotID % columns;
        var row = slotID / columns;

        TryAddFrontierSlot(slotID - 1, column > 0, occupied, frontier, frontierSet);
        TryAddFrontierSlot(slotID + 1, column + 1 < columns, occupied, frontier, frontierSet);
        TryAddFrontierSlot(slotID - columns, row > 0, occupied, frontier, frontierSet);
        TryAddFrontierSlot(slotID + columns, row + 1 < rows, occupied, frontier, frontierSet);
    }

    private static void TryAddFrontierSlot(
        int slotID,
        bool isValid,
        List<bool> occupied,
        List<int> frontier,
        HashSet<int> frontierSet)
    {
        if (!isValid || occupied[slotID] || !frontierSet.Add(slotID))
        {
            return;
        }

        frontier.Add(slotID);
    }

    private static int FindNextOccupiedSlotInRow(
        List<bool> occupiedSlots,
        int columns,
        int row,
        int column)
    {
        for (var nextColumn = column + 1; nextColumn < columns; nextColumn++)
        {
            var candidateSlotID = row * columns + nextColumn;

            if (candidateSlotID >= occupiedSlots.Count)
            {
                break;
            }

            if (occupiedSlots[candidateSlotID])
            {
                return candidateSlotID;
            }
        }

        return -1;
    }

    private static int FindNextOccupiedSlotInColumn(
        List<bool> occupiedSlots,
        int columns,
        int rows,
        int row,
        int column)
    {
        for (var nextRow = row + 1; nextRow < rows; nextRow++)
        {
            var candidateSlotID = nextRow * columns + column;

            if (candidateSlotID >= occupiedSlots.Count)
            {
                break;
            }

            if (occupiedSlots[candidateSlotID])
            {
                return candidateSlotID;
            }
        }

        return -1;
    }

    public Intersection AddIntersection(Vector2 position)
    {
        var intersection = new Intersection(position);
        _intersections.Add(intersection);
        return intersection;
    }

    private bool AddBidirectionalRoad(int fromID, int toID)
    {
        if (fromID == toID)
        {
            return false;
        }

        if (HasRoadBetween(fromID, toID))
        {
            return false;
        }

        var from = _intersections[fromID];
        var to = _intersections[toID];
        var length = Vector2.Distance(from.Position, to.Position);

        var outbound = new Road(fromID, toID, length);
        var inbound = new Road(toID, fromID, length);

        _roads.Add(outbound);
        _roads.Add(inbound);
        from.AddConnectedRoad(outbound);
        to.AddConnectedRoad(inbound);
        return true;
    }

    private bool HasRoadBetween(int fromID, int toID)
    {
        foreach (var road in _roads)
        {
            if (road.FromIdx == fromID && road.ToIdx == toID)
            {
                return true;
            }
        }

        return false;
    }
}
