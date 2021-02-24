using FigmaSharp.Models;
using System.Linq;
using System.Windows.Controls;
using FigmaSharp.Controls;
using System.Collections.Generic;
using System;
using System.Windows;

namespace FigmaSharp.Extensions
{
    internal static class GridDefinitionNodeExtensions
    {
        internal const string rowDefinitionsNodeName = "RowDefinitions";

        internal const string columnDefinitionsNodeName = "ColumnDefinitions";

        internal const string gridDefinitionNodeName = "GridDefinitions";

        public static bool IsGridDefinition(this FigmaNode figmaNode)
        {
            var gridDefinitionNode = figmaNode.GetGridDefinitionsNode(); 
                
            if (gridDefinitionNode != null 
                && (gridDefinitionNode.TryGetChildPropertyValue(rowDefinitionsNodeName, out var rowValue) && rowValue == rowDefinitionsNodeName)
                || (gridDefinitionNode.TryGetChildPropertyValue(columnDefinitionsNodeName, out var columnValue) && columnValue == columnDefinitionsNodeName))
            {
                return true;
            }
            return false;
        }

        public static bool isGridDefinitionsEnabled(this FigmaNode figmaNode)
        {
            return GetGridDefinitionsNode(figmaNode)?.visible ?? false;
        }

        public static FigmaNode GetGridDefinitionsNode(this FigmaNode figmaNode)
        {
            return (figmaNode as IFigmaNodeContainer)?.children?.FirstOrDefault(s => s.GetNodeTypeName() == gridDefinitionNodeName);
        }

        public static FigmaNode GetRowDefinitionsNode(this FigmaNode figmaNode)
        {
            return (figmaNode as IFigmaNodeContainer)?.children?.FirstOrDefault(s => s.GetNodeTypeName() == rowDefinitionsNodeName);
        }

        public static FigmaNode GetColumnDefinitionsNode(this FigmaNode figmaNode)
        {
            return (figmaNode as IFigmaNodeContainer)?.children?.FirstOrDefault(s => s.GetNodeTypeName() == columnDefinitionsNodeName);
        }

        public static bool TrySearchRowDefinitions(this FigmaNode figmaNode, out List<RowDefinition> rowDefinitions)
        {
            var gridDefinitionsNode = figmaNode.GetRowDefinitionsNode();
            if (gridDefinitionsNode != null)
            {
                var definitions = new List<RowDefinition>();
                foreach (FigmaText node in gridDefinitionsNode.GetChildren().OfType<FigmaText>())
                {
                    if (node.name.Contains(ComponentString.ROW_DEFINITION))
                    {
                        var rowDefinition = new RowDefinition();
                        rowDefinition.Height = GetGridLength(node);
                        definitions.Add(rowDefinition);
                    }
                }
                rowDefinitions = definitions;
                return true;
            }
            rowDefinitions = null;
            return false;
        }

        public static bool TrySearchColumnDefinitions(this FigmaNode figmaNode, out List<ColumnDefinition> columnDefinitions)
        {
            var gridDefinitionsNode = figmaNode.GetColumnDefinitionsNode();
            if (gridDefinitionsNode != null)
            {
                var definitions = new List<ColumnDefinition>();
                foreach (FigmaText node in gridDefinitionsNode.GetChildren().OfType<FigmaText>())
                {
                    if (node.name.Contains(ComponentString.COLUMN_DEFINITION))
                    {
                        var columnDefinition = new ColumnDefinition();
                        columnDefinition.Width = GetGridLength(node);
                        definitions.Add(columnDefinition);
                    }
                }
                columnDefinitions = definitions;
                return true;
            }
            columnDefinitions = null;
            return false;
        }

        // TODO: add grid row and column position retrieval

        private static GridLength GetGridLength(FigmaNode figmaNode)
        {
            var options = figmaNode.name.Split('|');
            if (options.Length > 1)
            {
                if (options[1].ToLower().Contains(ComponentString.AUTO))
                {
                    return new GridLength(0, GridUnitType.Auto);
                }
                else if (options[1].ToLower().Contains(ComponentString.STAR))
                {
                    if (options[1].Length > 1)
                    {
                        var length = options[1].Split(ComponentString.STAR[0]);
                        if (int.TryParse(length[0], out var result))
                        {
                            return new GridLength(result, GridUnitType.Star);
                        }
                    }
                    return new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    if (int.TryParse(options[1], out var result))
                    {
                        return new GridLength(result, GridUnitType.Pixel);
                    }
                }
            }
            return new GridLength(1, GridUnitType.Star);
        }
        
    }
}
