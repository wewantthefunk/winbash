using System;
using System.Collections.Generic;
using System.IO;

namespace bash.dotnet
{
    public class TreeNode
    {
        public string DirectoryPath { get; }
        public TreeNode? Parent { get; set; }
        public List<TreeNode> Children { get; }

        public TreeNode(string directoryPath)
        {
            DirectoryPath = directoryPath;
            Children = new List<TreeNode>();
        }

        public void AddChild(TreeNode node)
        {
            Children.Add(node);
            node.Parent = this;
        }
    }

    public class DirectoryTree
    {
        public TreeNode Root { get; private set; }

        public DirectoryTree(string rootDirectoryPath)
        {
            Root = new TreeNode(rootDirectoryPath);
            BuildTree(Root);
        }

        private void BuildTree(TreeNode currentNode)
        {
            try
            {
                var subDirectories = Directory.GetDirectories(currentNode.DirectoryPath);
                foreach (var dir in subDirectories)
                {
                    TreeNode childNode = new(dir);
                    currentNode.AddChild(childNode);

                    // Recursively build the tree for each subdirectory
                    BuildTree(childNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Log it or handle it: in case we don't have access to the directory
            }
        }

        public TreeNode? Find(string directoryPath)
        {
            return Find(Root, directoryPath);
        }

        private TreeNode? Find(TreeNode currentNode, string directoryPath)
        {
            if (currentNode.DirectoryPath.Equals(directoryPath, StringComparison.OrdinalIgnoreCase))
            {
                return currentNode;
            }

            foreach (var child in currentNode.Children)
            {
                var found = Find(child, directoryPath);
                if (found != null)
                {
                    return found;
                }
            }

            return null; // Not found
        }
    }
}
