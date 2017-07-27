
using AutoCompleteService.Common.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AutoCompleteService.Common.Models
{
    internal class TrieIndexSerializer
    {
		#region Serializer
        
		public static int Serialize(TrieNode rootNode, TrieIndexHeader trieIndexHeader, Stream index)
        {
            int processedNodeCount = 0;

            Queue<TrieNode> serializerQueue = new Queue<TrieNode>();
            serializerQueue.Enqueue(rootNode);

            TrieNode currentNode = null;
            BinaryWriter binaryWriter = new BinaryWriter(index);

            while (serializerQueue.Count > 0)
            {
                currentNode = serializerQueue.Dequeue();

                if (currentNode == null)
                    throw new InvalidDataException(string.Format("Value cannot be null ", processedNodeCount));

                long currentPositionOfStream = binaryWriter.BaseStream.Position;

               
                UInt16? characterIndex = TrieIndexHeaderCharacterReader.Instance.GetCharacterIndex(trieIndexHeader, currentNode.Character);
                if (characterIndex != null && characterIndex.HasValue)
                {
                    binaryWriter.Write(characterIndex.Value);
                }
                else
                {
                    binaryWriter.Write(Convert.ToUInt16(0));
                }

                binaryWriter.Write(currentNode.IsTerminal);
                
                BitArray baChildren = new BitArray(trieIndexHeader.COUNT_OF_CHARSET);
                if (currentNode.Children != null)
                {
                    foreach (var item in currentNode.Children)
                    {
                        UInt16? itemIndex = TrieIndexHeaderCharacterReader.Instance.GetCharacterIndex(trieIndexHeader, item.Key);
                        baChildren.Set(itemIndex.Value, true);
                    }
                }

                int[] childrenFlags = new int[trieIndexHeader.COUNT_OF_CHILDREN_FLAGS_IN_BYTES];
                baChildren.CopyToInt32Array(childrenFlags, 0);

                for (int i = 0; i < childrenFlags.Length; i++)
                {
                    binaryWriter.Write(childrenFlags[i]);
                }
                binaryWriter.Write(currentNode.ChildrenCount * trieIndexHeader.LENGTH_OF_STRUCT);


                if (currentNode.PositionOnTextFile.HasValue)
                {
                    binaryWriter.Write((uint)currentNode.PositionOnTextFile.Value);
                }
                else
                {
                    binaryWriter.Write((uint)0);
                }

                if (currentNode.Children != null)
                {
                    foreach (var childNode in currentNode.Children)
                    {
                        serializerQueue.Enqueue(childNode.Value);
                    }
                }

                ++processedNodeCount;
            }

            return processedNodeCount;
        }

        #endregion
    }
}