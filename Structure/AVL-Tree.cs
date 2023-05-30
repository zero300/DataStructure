using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Structure.Structure
{
    public class AVL_Node
    {
        public AVL_Node(float value)
        {
            Value = value;
        }
        /// <summary>
        /// 節點值
        /// </summary>
        public float Value;
        /// <summary>
        /// 高度
        /// </summary>
        public int Height 
        {
            get {
                return Math.Max(this.Left == null ? 0 : this.Left.Height + 1, this.Right == null ? 0 : this.Right.Height + 1);
            }
        }
        /// <summary>
        /// 左節點
        /// </summary>
        public AVL_Node? Left = null;
        /// <summary>
        /// 右節點
        /// </summary>
        public AVL_Node? Right = null;
        /// <summary>
        /// 父節點
        /// </summary>
        public AVL_Node? Parent = null;
    }

    public class AVL_Tree
    {
        public AVL_Node? root = null;

        /// <summary>
        /// 搜尋
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public AVL_Node? Search(float num)
        {
            AVL_Node? node = root;
            while(node != null)
            {
                // 找到值了
                if (node.Value == num) return node;
                // 判斷在左邊還右邊
                if (node.Value > num) node = node.Left;
                else node = node.Right;
            }
            return null;
        }
        /// <summary>
        /// 新增節點
        /// </summary>
        /// <param name="num"></param>
        /// <exception cref="Exception"></exception>
        public void Insert(float num)
        {
            AVL_Node NewNode = new AVL_Node(num);
            // 根節點為 null
            if (root == null)
            {
                root = NewNode;
                return;
            }
            AddNewNode(root, NewNode);
            RootCheck();
        }
        /// <summary>
        /// 新增節點 遞迴
        /// </summary>
        /// <param name="node"></param>
        private void AddNewNode(AVL_Node node , AVL_Node newNode)
        {
            if (node == null) return;

            // 添加節點
            if(node.Value < newNode.Value){
                if (node.Right == null)
                {
                    node.Right = newNode;
                    newNode.Parent = node;
                }
                else AddNewNode(node.Right, newNode);
            }
            if(node.Value > newNode.Value){
                if (node.Left == null)
                {
                    node.Left = newNode;
                    newNode.Parent = node;
                }
                else AddNewNode(node.Left, newNode);
            }

            AVL_Node? Left = node.Left;
            AVL_Node? Right = node.Right;

            int LeftHeight = GetNodeHeight(Left);
            int RightHeight = GetNodeHeight(node.Right);

            // 添加完節點 判斷平衡
            // 左節點重於右節點
            if(LeftHeight - RightHeight > 1) {
                // 判斷要不要雙重旋轉
                if(Left != null && GetNodeHeight(Left.Right) > GetNodeHeight(Left.Left) )
                {
                    LeftRotate(Left);
                    RightRotate(node);
                }
                else
                {
                    RightRotate(node);
                }
            }
            // 右節點重於左節點
            if (RightHeight - LeftHeight > 1)
            {
                // 判斷要不要雙重旋轉
                if (Right != null && GetNodeHeight(Right.Left) > GetNodeHeight(Right.Right))
                {
                    RightRotate(Right);
                    LeftRotate(node);
                }
                else
                {
                    LeftRotate(node);
                }
            }

            //可能要加判斷高度?
        }
        /// <summary>
        /// 刪除節點
        /// </summary>
        /// <param name="num"></param>
        public bool Remove(float num)
        {
            if (root == null) return false;

            // 找出要刪除的節點
            AVL_Node? Target = Search(num);
            if (Target == null) return false;

            // TODO : 要做 FIX
            // 如果底下為空，則直接刪除(葉節點)
            if(Target.Left ==null && Target.Right == null)
            {
                if(Target.Parent != null)
                {
                    // 把指向自己的部分設為null
                    if (Target.Parent.Left == Target) Target.Parent.Left = null;
                    else Target.Parent.Right = null;

                    DeleteFix(Target.Parent);
                }
                else
                {
                    // 代表 沒有Parent 沒有子節點
                    // 代表為 root
                    // 不用Check了
                    root = null;
                }

            }
            else if (Target.Left != null && Target.Right != null)// 兩邊皆有節點
            {
                // 找出前繼節點(或後繼節點)
                // 先往左找一格 接下來一直往右(也可以反過來 先往右 在一直向左)
                AVL_Node temp = Target.Left;
                while(temp.Right != null)
                {
                    temp = temp.Right;
                }

                // 找到後 使Target 值為找到得那個 
                Target.Value = temp.Value;
                // 先判斷自己是 Parent 的左節點 or 右節點
                if(temp.Parent.Left == temp)
                {// 左節點
                    // 如果刪除的前(後)繼節點 的左(右)節點還有節點
                    if (temp.Left != null) temp.Parent.Left = temp.Left;
                    else temp.Parent.Left = null;
                }else if (temp.Parent.Right == temp)
                {// 右節點
                    // 如果刪除的前(後)繼節點 的左(右)節點還有節點
                    if (temp.Left != null) temp.Parent.Right = temp.Left;
                    else temp.Parent.Right = null;
                    
                }
                DeleteFix(temp.Parent);
            }
            else
            {// 只有一邊有節點
                if(Target.Left != null) Target.Value = Target.Left.Value;
                else if (Target.Right != null) Target.Value = Target.Right.Value;
                DeleteFix(Target);
            }

            RootCheck();
            return true;
        }
        /// <summary>
        /// 修復Delete造成的不平衡
        /// </summary>
        /// <param name="node"></param>
        private void DeleteFix(AVL_Node node)
        {
            AVL_Node next = node.Parent;
            while (node != null)
            {
                AVL_Node? Left = node.Left;
                AVL_Node? Right = node.Right;

                int LeftHeight = GetNodeHeight(Left);
                int RightHeight = GetNodeHeight(node.Right);
                // 添加完節點 判斷平衡
                // 左節點重於右節點
                if (LeftHeight - RightHeight > 1)
                {
                    // 判斷要不要雙重旋轉
                    if (Left != null && GetNodeHeight(Left.Right) > GetNodeHeight(Left.Left))
                    {
                        LeftRotate(Left);
                        RightRotate(node);
                    }
                    else
                    {
                        RightRotate(node);
                    }
                }
                // 右節點重於左節點
                if (RightHeight - LeftHeight > 1)
                {
                    // 判斷要不要雙重旋轉
                    if (Right != null && GetNodeHeight(Right.Left) > GetNodeHeight(Right.Right))
                    {
                        RightRotate(Right);
                        LeftRotate(node);
                    }
                    else
                    {
                        LeftRotate(node);
                    }
                }
                node = next;
                if(node != null) next = node.Parent;
            }
        }
        /// <summary>
        /// 顯示樹
        /// </summary>
        /// <returns></returns>
        public string ShowTree()
        {
            if (root == null) return "No Tree";

            StringBuilder sb = new StringBuilder();
            Show(ref sb, root);
            return sb.ToString();
        }
        /// <summary>
        /// 遞迴
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="node"></param>
        private void Show(ref StringBuilder stringBuilder, AVL_Node node)
        {
            if (node.Left != null) Show(ref stringBuilder, node.Left);
            if (node.Right != null) Show(ref stringBuilder, node.Right);

            stringBuilder.Append(node.Value);
            stringBuilder.Append(',');
        }
        /// <summary>
        /// 右旋轉 LL
        /// </summary>
        /// <param name="node"></param>
        private void RightRotate(AVL_Node node)
        {
            Console.WriteLine("Right Rotate On " + node.Value);
            if (node.Left == null) return;

            AVL_Node temp_left = node.Left;
            AVL_Node? temp_left_right = node.Left.Right;

            Console.WriteLine($"temp_left = {temp_left.Value} , temp_left_right = {(temp_left_right == null ? "Null" : temp_left_right.Value)}");
            if (node.Parent != null)
            {
                Console.WriteLine("Nit null");
                if (node.Parent.Right == node)
                {
                    node.Parent.Right = temp_left;
                    Console.WriteLine("Parent Right");
                }
                else
                {
                    node.Parent.Left = temp_left;
                    Console.WriteLine("Parent Left");
                }
            }
            temp_left.Parent = node.Parent;

            node.Parent = temp_left;
            temp_left.Right = node;

            node.Left = temp_left_right;
            if (temp_left_right != null) temp_left_right.Parent = node;
        }
        /// <summary>
        /// 左旋轉 RR
        /// </summary>
        /// <param name="node"></param>
        private void LeftRotate(AVL_Node node)
        {
            Console.WriteLine("Left Rotate On " + node.Value);
            if (node.Right == null) return;

            AVL_Node temp_right = node.Right;
            AVL_Node? temp_right_left = node.Right.Left;
            if (node.Parent != null)
            {
                // 判斷是parent的左還右節點
                if (node.Parent.Left == node) node.Parent.Left = temp_right;
                else node.Parent.Right = temp_right;
            }

            temp_right.Parent = node.Parent;

            node.Parent = temp_right;
            temp_right.Left = node;

            node.Right = temp_right_left;
            if (temp_right_left != null) temp_right_left.Parent = node;
        }
        /// <summary>
        /// 重製根節點
        /// 旋轉可能導致根節點變化
        /// </summary>
        private void RootCheck()
        {
            if (root == null) return;
            while (root.Parent != null)
            {
                root = root.Parent;
            }
        }
        /// <summary>
        /// 判斷Node高度
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private int GetNodeHeight(AVL_Node? node)
        {
            return node == null ? -1 : node.Height;
        }
    }
}
