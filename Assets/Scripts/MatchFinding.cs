#pragma warning disable IDE0130 

namespace Texell.CandyCoolSummer
{
    using System.Collections.Generic;
    using Texell.CoreModule;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public class MatchFinding : MonoBehaviour
    {
        private readonly List<Vector3Int> _matchedCells = new();
        private Dictionary<Vector3Int, Cell> _boardCells;
        private readonly List<Match> _tickingMatch = new();
        private readonly AssetManager _assetManager = AssetManager.Instance;

        public MatchShape LineFiveShape;
        public MatchShape LineFourHShape;
        public MatchShape LineFourVShape;
        public MatchShape TShape;
        public MatchShape LShape;
        public MatchShape SquareShape;

        public List<Match> TickingMatch => _tickingMatch;
        public List<Vector3Int> MatchedCells => _matchedCells;

        public void Init(Board board)
        {
            _boardCells = board.BoardCells;
        }

        /// <summary>
        /// This will return true if a match was found.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="direction">If no direction pass in Vector3Int.zero</param>
        /// <param name="createMatch"></param>
        /// <returns></returns>
        public bool FindMatch(Vector3Int startPos, Vector3Int direction, bool createMatch = true)
        {
            MatchType matchType = MatchType.LineThree;
            List<Vector3Int> matchingCells = new();

            if (!_boardCells.TryGetValue(startPos, out var startCell) || startCell.ContainingCandy == null)
            {
                return false;
            }

            // We ignore that candy if it's already part of another match.
            if (startCell.ContainingCandy.CurrentMatch != null)
                return false;

            Vector3Int[] offsets = new[]
            {
                Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left
            };

            // First find all the connected candy of the same type.
            List<Vector3Int> candyList = new();
            List<Vector3Int> visited = new();

            Queue<Vector3Int> nodes = new();
            nodes.Enqueue(startPos);

            while (nodes.Count > 0)
            {
                var currentPos = nodes.Dequeue();

                candyList.Add(currentPos);
                visited.Add(currentPos);

                foreach (var dir in offsets)
                {
                    var nextPos = currentPos + dir;

                    if (visited.Contains(nextPos))
                        continue;

                    if (_boardCells.TryGetValue(nextPos, out var nextCell)
                        && nextCell.CanMatch()
                        && nextCell.ContainingCandy.Type == startCell.ContainingCandy.Type)
                    {
                        nodes.Enqueue(nextPos);
                    }
                }
            }

            //-- Get list cells of shape match.
            List<Vector3Int> lineFiveShapeMatch = new();
            List<Vector3Int> lineFourHShapeMatch = new();
            List<Vector3Int> lineFourVShapeMatch = new();
            List<Vector3Int> tShapeMatch = new();
            List<Vector3Int> lShapeMatch = new();
            List<Vector3Int> squareShapeMatch = new();

            var lineFiveFit = LineFiveShape.FitIn(candyList, ref lineFiveShapeMatch);
            var lineFourHFit = LineFourHShape.FitIn(candyList, ref lineFourHShapeMatch);
            var lineFourVFit = LineFourVShape.FitIn(candyList, ref lineFourVShapeMatch);
            var tFit = TShape.FitIn(candyList, ref tShapeMatch);
            var lFit = LShape.FitIn(candyList, ref lShapeMatch);
            var squareFit = SquareShape.FitIn(candyList, ref squareShapeMatch);

            //-- Now we build a list of all line of 3+ candies.
            List<List<Vector3Int>> lineList = new();

            foreach (var cellPos in candyList)
            {
                //for each dir (up/down/left/right) if there is no gem in that dir, that mean this could be the start of
                //a matching line, so we check in the opposite direction till we have no more gem
                foreach (var dir in offsets)
                {
                    if (!candyList.Contains(cellPos + dir))
                    {
                        var currentList = new List<Vector3Int>() { cellPos };
                        var next = cellPos - dir;
                        while (candyList.Contains(next))
                        {
                            currentList.Add(next);
                            next -= dir;
                        }

                        if (currentList.Count >= 3)
                        {
                            lineList.Add(currentList);
                        }
                    }
                }
            }

            // Determine match type.
            foreach (var line in lineList)
            {
                if (line.Count == 5)
                {
                    matchType = MatchType.LineFive;
                    break;
                }
            }

            List<Vector3Int> shapeList = new();
            if (lineFiveFit)
            {
                foreach (var pos in lineFiveShapeMatch)
                {
                    if (!shapeList.Contains(pos))
                    {
                        shapeList.Add(pos);
                    }
                }

                matchType = MatchType.LineFive;
                Debug.Log("lineFiveFit===============");
            }

            if (lineFourHFit)
            {
                foreach (var pos in lineFourHShapeMatch)
                {
                    if (!shapeList.Contains(pos))
                    {
                        shapeList.Add(pos);
                    }
                }

                if (!lineFiveFit)
                {
                    matchType = MatchType.LineFourH;
                }
                Debug.Log("lineFourHFit===============");
            }

            if (lineFourVFit)
            {
                foreach (var pos in lineFourVShapeMatch)
                {
                    if (!shapeList.Contains(pos))
                    {
                        shapeList.Add(pos);
                    }
                }

                if (!lineFiveFit)
                {
                    matchType = MatchType.LineFourV;
                }
                Debug.Log("lineFourVFit===============");
            }

            if (tFit)
            {
                foreach (var pos in tShapeMatch)
                {
                    if (!shapeList.Contains(pos))
                    {
                        shapeList.Add(pos);
                    }
                }

                if (matchType != MatchType.LineFive)
                {
                    matchType = MatchType.TLShape;
                }

                Debug.Log("tFit===============");
            }
            else if (lFit)
            {
                foreach (var pos in lShapeMatch)
                {
                    if (!shapeList.Contains(pos))
                    {
                        shapeList.Add(pos);
                    }
                }

                if (matchType != MatchType.LineFive)
                {
                    matchType = MatchType.TLShape;
                }

                Debug.Log("lFit===============");
            }

            if (squareFit)
            {
                foreach (var pos in squareShapeMatch)
                {
                    if (!shapeList.Contains(pos))
                    {
                        shapeList.Add(pos);
                    }
                }

                if (!lineFiveFit && !lineFourHFit && !lineFourVFit && !tFit && !lFit)
                {
                    matchType = MatchType.SquareShape;
                }

                Debug.Log("squareFit===============");
            }

            bool matchFound;
            if (lineList.Count == 0 && shapeList.Count == 0)
            {
                matchFound = false;
            }
            else
            {
                matchFound = true;

                foreach (var line in lineList)
                {
                    foreach (var pos in line)
                    {
                        if (!matchingCells.Contains(pos))
                        {
                            matchingCells.Add(pos);
                        }
                    }
                }

                foreach (var pos in shapeList)
                {
                    if (!matchingCells.Contains(pos))
                    {
                        matchingCells.Add(pos);
                    }
                }

            }

            if (createMatch)
            {
                if (matchFound)
                {
                    var finalMatch = CreateCustomMatch(matchType, startPos, direction);
                    foreach (var pos in matchingCells)
                    {
                        if (_boardCells[pos].CanDelete())
                        {
                            finalMatch.AddCandy(_boardCells[pos].ContainingCandy);
                        }
                    }
                }
            }
            else
            {
                if (matchFound)
                {
                    foreach (var pos in matchingCells)
                    {
                        _matchedCells.Add(pos);
                    }
                }
            }

            return matchFound;
        }

        Match CreateCustomMatch(MatchType type, Vector3Int point, Vector3Int direction)
        {
            var match = new Match()
            {
                DeletionTimer = 0.0f,
                Type = type,
                CombinedPoint = point,
                Direction = direction
            };

            var candyType = _boardCells[match.CombinedPoint].ContainingCandy.Type;
            switch (match.Type)
            {
                case MatchType.LineFourH:
                    if (match.Direction == Vector3Int.left || match.Direction == Vector3Int.right || match.Direction == Vector3Int.zero)
                    {
                        if (candyType == Candy.ColorType.BLUE)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_BLUE].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.YELLOW)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_YELLOW].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.RED)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_RED].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.GREEN)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_GREEN].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.PURPLE)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_PURPLE].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.PINK)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_PINK].GetComponent<Candy>();
                        }
                    }
                    else if (match.Direction == Vector3Int.up || match.Direction == Vector3Int.down)
                    {
                        if (candyType == Candy.ColorType.BLUE)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_BLUE].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.YELLOW)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_YELLOW].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.RED)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_RED].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.GREEN)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_GREEN].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.PURPLE)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_PURPLE].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.PINK)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_PINK].GetComponent<Candy>();
                        }
                    }
                    break;
                case MatchType.LineFourV:
                    if (match.Direction == Vector3Int.left || match.Direction == Vector3Int.right)
                    {
                        if (candyType == Candy.ColorType.BLUE)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_BLUE].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.YELLOW)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_YELLOW].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.RED)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_RED].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.GREEN)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_GREEN].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.PURPLE)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_PURPLE].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.PINK)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.H_PINK].GetComponent<Candy>();
                        }
                    }
                    else if (match.Direction == Vector3Int.up || match.Direction == Vector3Int.down || match.Direction == Vector3Int.zero)
                    {
                        if (candyType == Candy.ColorType.BLUE)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_BLUE].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.YELLOW)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_YELLOW].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.RED)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_RED].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.GREEN)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_GREEN].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.PURPLE)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_PURPLE].GetComponent<Candy>();
                        }
                        else if (candyType == Candy.ColorType.PINK)
                        {
                            match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.V_PINK].GetComponent<Candy>();
                        }
                    }
                    break;
                case MatchType.LineFive:
                    match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.CHOCOLATE_MILK].GetComponent<Candy>();
                    break;
                case MatchType.TLShape:
                    if (candyType == Candy.ColorType.BLUE)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.STING_BLUE].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.YELLOW)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.STING_YELLOW].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.RED)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.STING_RED].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.GREEN)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.STING_GREEN].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.PURPLE)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.STING_PURPLE].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.PINK)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.STING_PINK].GetComponent<Candy>();
                    }
                    break;
                case MatchType.SquareShape:
                    if (candyType == Candy.ColorType.BLUE)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_BLUE].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.YELLOW)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_YELLOW].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.RED)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_RED].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.GREEN)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_GREEN].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.PURPLE)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_PURPLE].GetComponent<Candy>();
                    }
                    else if (candyType == Candy.ColorType.PINK)
                    {
                        match.CombinedPrefab = _assetManager.CandyPrefabs[(int)CandyIndex.SWIRL_PINK].GetComponent<Candy>();
                    }
                    break;
            }

            _tickingMatch.Add(match);

            return match;
        }
    }
}