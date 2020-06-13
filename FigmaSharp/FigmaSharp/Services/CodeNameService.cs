using System;
using System.Collections.Generic;
using FigmaSharp.Converters;

namespace FigmaSharp.Services
{
    public interface ICodeNameService
    {
        bool NodeHasName(CodeNode node);
        string GenerateName(CodeNode node, CodeNode parent, NodeConverter converter);
        void Clear();
    }

    public class CodeNameService : ICodeNameService
    {
        const string ViewIdentifier = "View";
        const string IdentifierStart = "Figma";
        const string IdentifierEnd = "Converter";

        internal const string DefaultViewName = "view";

        protected Dictionary<string, int> identifiers = new Dictionary<string, int>();

        public string GenerateName(CodeNode node, CodeNode parent, NodeConverter converter)
        {
            if (!TryGetCodeViewName(node, parent, converter, out string identifier))
            {
                identifier = DefaultViewName;
            }

            //we store our name to don't generate dupplicates
            var lastIndex = GetLastInsertedIndex(identifier);
            if (lastIndex >= 0)
            {
                identifiers.Remove(identifier);
            }
            lastIndex++;

            identifiers.Add(identifier, lastIndex);

            //node.Name = identifier;
            if (lastIndex > 0)
            {
                identifier += lastIndex;
            }

            return identifier;
        }

        public bool NodeHasName(CodeNode node)
        {
            return node.HasName;
        }

        protected virtual bool TryGetCodeViewName(CodeNode node, CodeNode parent, NodeConverter converter, out string identifier)
        {
            try
            {
                identifier = converter.GetType().Name;
                if (identifier.StartsWith(IdentifierStart))
                {
                    identifier = identifier.Substring(IdentifierStart.Length);
                }

                if (identifier.EndsWith(IdentifierEnd))
                {
                    identifier = identifier.Substring(0, identifier.Length - IdentifierEnd.Length);
                }

                identifier = char.ToLower(identifier[0]) + identifier.Substring(1) + ViewIdentifier;

                return true;
            }
            catch (Exception)
            {
                identifier = null;
                return false;
            }
        }

        int GetLastInsertedIndex(string identifier)
        {
            if (!identifiers.TryGetValue(identifier, out int data))
            {
                return -1;
            }
            return data;
        }

        public void Clear()
        {
            identifiers.Clear();
        }
    }
}
