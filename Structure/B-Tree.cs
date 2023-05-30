using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Structure.Structure
{
    public class B_Tree_Node
    {
        private int _maxKeyLength;
        private int _minKeyLength;

        public B_Tree_Node(int degree, B_Tree_Node? parent)
        {
            _maxKeyLength = degree;
            _minKeyLength = (int)Math.Ceiling(degree / 2d) - 1;
            Keys = new int?[degree];
            Parent = parent;
            Children = new B_Tree_Node?[degree + 1];
        }

        public int?[] Keys;
        public int KeyCount { get; private set; }
        public B_Tree_Node? Parent;
        public B_Tree_Node?[] Children;

        public bool IsRoot => Parent == null;
        public bool IsLeaf => Children[0] == null;

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="key"></param>
        public void AddKey(int key)
        {
            Insert(key);
            SplitIfNecessary();
        }
        /// <summary>
        /// 刪除某點
        /// </summary>
        /// <param name="index">位於第幾位</param>
        public void RemoveAt(int index)
        {
            // 如果是葉節點 直接刪除
            // 在判斷一下就好
            if (this.IsLeaf)
            {
                this.DeleteAt(index);

                this.MergeIfNecessary();
            }
            else
            {
                // 非葉節點 先找出替代點的值
                // 把原本的值 替換成 替代值 
                // 再刪除替代點
                this.Keys[index] = default;

                var leftLeaf = FindLeftLeaf();

                this.Keys[index] = leftLeaf.Keys[leftLeaf.KeyCount - 1];

                leftLeaf.RemoveAt(leftLeaf.KeyCount - 1);
            }

            // 找左節點 最靠近這個數字的
            B_Tree_Node FindLeftLeaf()
            {
                var leaf = this.Children[index];

                while (!leaf.IsLeaf)
                {
                    leaf = leaf.Children[leaf.KeyCount];
                }

                return leaf;
            }
        }
        // -------------------------------- static 
        /// <summary>
        /// 設置子節點
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="childindex"></param>
        private static void SetChild(B_Tree_Node parent, B_Tree_Node? child, int childindex)
        {
            if (child != null)
            {
                child.Parent = parent;
            }
            parent.Children[childindex] = child;
        }
        // -------------------------------- Private Function
        /// <summary>
        /// 插入節點
        /// </summary>
        /// <param name="number"></param>
        private int Insert(int number)
        {
            int index = 0;
            // 先找出位置
            for (; index < this.KeyCount; index++)
            {
                int compare = number.CompareTo(this.Keys[index]);
                if (compare == 0) return -1;
                if (compare < 1) break;
            }

            if(this.KeyCount > 0 && index < this.KeyCount)
            {
                for (int i = this.KeyCount ; i >index ; i--)
                {
                    this.Keys[i] = this.Keys[i- 1];
                    this.Children[i + 1] = this.Children[i];
                }
                this.Children[index + 1] = this.Children[index];
                this.Children[index] = default;
            }
            this.Keys[index] = number;
            this.KeyCount++;

            return index;
        }
        /// <summary>
        /// 必要的分割
        /// </summary>
        private void SplitIfNecessary()
        {
            if (this.KeyCount < this._maxKeyLength) return;

            var medianIndex = (int)Math.Ceiling(this.KeyCount / 2d) - 1;

            var index = 0;

            var leftNode = new B_Tree_Node(this._maxKeyLength, this);
            
            // 最左邊的Child 直接繼承給 新的最左邊的 Node
            SetChild(leftNode, this.Children[index], 0);

            // 將最左邊 ~ 中間的值給予 左邊的node
            // 並且將其之間的Child也賦予
            for (var i = index; index < medianIndex; index++)
            {
                leftNode.Keys[i] = this.Keys[index];
                leftNode.KeyCount++;

                SetChild(leftNode, this.Children[index + 1], i + 1);

                i++;
            }

            // 跳過中間的值
            index++;

            // 跟上面一樣 不過是右邊
            var rightNode = new B_Tree_Node(this._maxKeyLength, this);

            SetChild(rightNode, this.Children[index], 0);

            for (var i = 0; index < this.KeyCount; index++)
            {
                rightNode.Keys[i] = this.Keys[index];
                rightNode.KeyCount++;

                SetChild(rightNode, this.Children[index + 1], i + 1);

                i++;
            }

            var medianKey = this.Keys[medianIndex];

            if (this.IsRoot)
            {
                Array.Clear(this.Keys, 0, this.Keys.Length);
                Array.Clear(this.Children, 0, this.Children.Length);

                this.Keys[0] = medianKey;
                this.Children[0] = leftNode;
                this.Children[1] = rightNode;

                this.KeyCount = 1;
            }
            else
            {
                var keyIndexInParent = this.Parent.Insert(medianKey.Value);

                SetChild(this.Parent, leftNode, keyIndexInParent);
                SetChild(this.Parent, rightNode, keyIndexInParent + 1);

                this.Parent.SplitIfNecessary();
            }
        }
        /// <summary>
        /// 刪除某值
        /// </summary>
        /// <param name="index"></param>
        private void DeleteAt(int index)
        {
            this.KeyCount--;
            for (int i = index; i < this.KeyCount; i++)
            {
                this.Keys[i] = this.Keys[i + 1];
            }
            this.Keys[this.KeyCount] = default;
        }
        /// <summary>
        /// 找出自己是Parent的第幾個Child
        /// </summary>
        /// <returns></returns>
        private int FindChildIndexInParent()
        {
            for (var i = 0; i < this.Parent.Children.Length; i++)
            {
                if (this.Parent.Children[i] == this) return i;
            }

            return -1;
        }
        /// <summary>
        /// 因刪除值會導致變少，所以可能會需要合併
        /// </summary>
        private void MergeIfNecessary()
        {
            if (this.IsRoot) return;
            if (this.KeyCount >= this._minKeyLength) return;

            var childIndexInParent = this.FindChildIndexInParent();

            // 兄弟節點
            var leftSibling = childIndexInParent > 0 ? this.Parent.Children[childIndexInParent - 1] : default;
            var rightSibling = childIndexInParent < this._maxKeyLength ? this.Parent.Children[childIndexInParent + 1] : default;

            // 如果兄弟節點的數量 > 最小節點數量的話
            if (leftSibling != null && leftSibling.KeyCount > this._minKeyLength)
            {
                var parentKeyIndex = childIndexInParent - 1;

                if (this.KeyCount == 0)
                {
                    this.Children[1] = this.Children[0];
                }

                // 將Parent的一個Key給自己
                // 且將兄弟節點的值給Parent
                this.Insert(this.Parent.Keys[parentKeyIndex].Value);
                this.Parent.Keys[parentKeyIndex] = leftSibling.Keys[leftSibling.KeyCount - 1];
                
                // 將兄弟節點最大的Child節點設給當前節點
                SetChild(this, leftSibling.Children[leftSibling.KeyCount], 0);

                // 由於把值給了Parent 所以將其值刪除
                leftSibling.Children[leftSibling.KeyCount] = default;
                leftSibling.DeleteAt(leftSibling.KeyCount - 1);
            }
            else if (rightSibling != null && rightSibling.KeyCount > this._minKeyLength)
            {// 跟上面差不多 只是是右邊
                var parentKeyIndex = childIndexInParent;

                this.Insert(this.Parent.Keys[parentKeyIndex].Value);
                this.Parent.Keys[parentKeyIndex] = rightSibling.Keys[0];

                SetChild(this, rightSibling.Children[0], this.KeyCount);

                for (var i = 0; i < rightSibling.KeyCount; i++)
                {
                    rightSibling.Children[i] = rightSibling.Children[i + 1];
                }

                rightSibling.Children[rightSibling.KeyCount] = rightSibling.Children[rightSibling.KeyCount + 1];
                rightSibling.DeleteAt(0);
            }
            else // 如果兩個都不符合
            {
                var parentKeyIndex = leftSibling != null ? childIndexInParent - 1 : childIndexInParent;

                var mergedNode = leftSibling ?? this;
                var manureNode = mergedNode == this ? rightSibling : this;

                mergedNode.Keys[mergedNode.KeyCount] = this.Parent.Keys[parentKeyIndex];
                mergedNode.KeyCount++;

                SetChild(mergedNode, manureNode.Children[0], mergedNode.KeyCount);

                // 因為現在節點不符合
                // 故將現在節點的Keys 跟 Child 移到要merge的Node
                for (var i = 0; i < manureNode.KeyCount; i++)
                {
                    mergedNode.Keys[mergedNode.KeyCount] = manureNode.Keys[i];
                    mergedNode.KeyCount++;

                    SetChild(mergedNode, manureNode.Children[i + 1], mergedNode.KeyCount);
                }

                // 把Parent的Keys進行調整
                // 因為有一個給Merge Node了
                for (var i = parentKeyIndex; i < this.Parent.KeyCount; i++)
                {
                    this.Parent.Children[i] = this.Parent.Children[i + 1];
                }

                this.Parent.Children[this.Parent.KeyCount] = default;
                this.Parent.Children[parentKeyIndex] = mergedNode;
                this.Parent.DeleteAt(parentKeyIndex);

                // 如果為root 或 parent 得 Keys數量為0
                if (this.Parent.IsRoot && this.Parent.KeyCount == 0)
                {
                    Array.Clear(this.Parent.Keys, 0, this.Parent.Keys.Length);
                    Array.Clear(this.Parent.Children, 0, this.Parent.Children.Length);

                    for (var i = 0; i < mergedNode.KeyCount; i++)
                    {
                        this.Parent.Keys[i] = mergedNode.Keys[i];

                        SetChild(this.Parent, mergedNode.Children[i], i);
                    }

                    SetChild(this.Parent, mergedNode.Children[mergedNode.KeyCount], mergedNode.KeyCount);

                    this.Parent.KeyCount = mergedNode.KeyCount;
                }
                else
                {
                    this.Parent.MergeIfNecessary();
                }
            }
        }
    }
    public class B_Tree
    {
        public B_Tree_Node root;
        public B_Tree(int degree) {
            root = new B_Tree_Node(degree, null);
        }
        public int Count { get; private set; }
        
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="number"></param>
        public void Add(int number)
        {
            B_Tree_Node leaf = FindLeaf(root, number);
            
            leaf.AddKey(number);
            
            Count++;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="number"></param>
        public void Remove(int number)
        {
            if(root == null) throw new ArgumentNullException(nameof(this.root));

            int index;
            B_Tree_Node? node = FindNode(root, number, out index);

            if (node == null) return;

            node.RemoveAt(index);
            this.Count--;

        }
        /// <summary>
        /// 尋找葉節點
        /// </summary>
        /// <param name="node">當前節點</param>
        /// <param name="number">要加入的數字</param>
        /// <returns></returns>
        private static B_Tree_Node FindLeaf(B_Tree_Node node ,int number)
        {
            if(node.IsLeaf) return node;
            
            for(int i = 0; i < node.KeyCount; i++)
            {
                if (number.CompareTo(node.Keys[i]) < 0) return FindLeaf(node.Children[i], number);
            }
            return FindLeaf(node.Children[node.KeyCount], number);
        }
        /// <summary>
        /// 找出節點
        /// </summary>
        /// <param name="node">當前節點</param>
        /// <param name="key">要找的值</param>
        /// <param name="index">值位於該Node的第幾位</param>
        /// <returns></returns>
        private static B_Tree_Node? FindNode(B_Tree_Node node, int key , out int index)
        {
            var i = 0;
            for(; i < node.KeyCount; i++) {
                var comparison = key.CompareTo(node.Keys[i]);
                if(comparison == 0)
                {
                    index = i;
                    return node;
                }else if(comparison < 0)
                {
                    return FindNode(node.Children[i], key, out index);
                }
            }

            if (!node.IsLeaf)
            {
                return FindNode(node.Children[i], key, out index);
            }

            index = -1;
            return null;
        }
    }
}
