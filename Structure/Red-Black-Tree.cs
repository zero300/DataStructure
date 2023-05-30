using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Data_Structure.Structure
{
    public enum Red_Black_Node_Color { Red, Black };
    public class Red_Black_Node {
        /// <summary>
        /// 節點值
        /// </summary>
        public float Value { get; }
        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 節點顏色
        /// </summary>
        public Red_Black_Node_Color Color { get; set; }
        /// <summary>
        /// 左節點
        /// </summary>
        public Red_Black_Node? Left { get; set; }
        /// <summary>
        /// 右節點
        /// </summary>
        public Red_Black_Node? Right { get; set; }
        /// <summary>
        /// 父節點
        /// </summary>
        public Red_Black_Node? Parent { get; set; }

        public Red_Black_Node(float value, Red_Black_Node_Color color){
            Value = value;
            Color = color;
        }
    }

    public class Red_Black_Tree
    {
        private Red_Black_Node? root = null ;
        /// <summary>
        /// 新增節點
        /// </summary>
        /// <param name="num"></param>
        /// <exception cref="Exception"></exception>
        public void Insert(float num)
        {
            Red_Black_Node NewNode = new Red_Black_Node(num, Red_Black_Node_Color.Red);

            // 根節點為 null
            if (root == null){
                root = NewNode;
                root.Color = Red_Black_Node_Color.Black;
            }
            else
            {
                // 插入判斷
                Red_Black_Node? next_node = root;
                Red_Black_Node? parent_node = null;
                // 找出要在插入在父節點上
                while(next_node != null)
                {
                    parent_node = next_node;
                    next_node = root.Value < num ? next_node.Right : next_node.Left ;
                }

                // 附值
                if (parent_node == null) throw new Exception("RBT Insert has some error");


                NewNode.Parent = parent_node;
                if (parent_node.Value > num) parent_node.Left = NewNode;
                else parent_node.Right = NewNode;
            }
            InsertFix(NewNode);
        }
        /// <summary>
        /// 修復新增節點問題
        /// </summary>
        /// <param name="node"></param>
        private void InsertFix(Red_Black_Node node)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void LRRotate(Red_Black_Node node)
        {

        }
        /// <summary>
        /// 右旋轉 LL
        /// </summary>
        /// <param name="node"></param>
        private void RightRotate(Red_Black_Node node)
        {
            Console.WriteLine("Right Rotate On" + node.Value);
            if (node.Left == null) return;

            Red_Black_Node temp_left = node.Left;
            Red_Black_Node? temp_left_right = node.Left.Right;

            if (node.Parent != null)
            {
                if (node.Parent.Right == node) node.Parent.Right = temp_left;
                else node.Parent.Left = temp_left;
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
        private void LeftRotate(Red_Black_Node node)
        {
            Console.WriteLine("Left Rotate On" + node.Value);
            if (node.Right == null) return;

            Red_Black_Node temp_right = node.Right;
            Red_Black_Node? temp_right_left = node.Right.Left;

            if(node.Parent != null)
            {
                // 判斷是parent的左還右節點
                if(node.Parent.Left == node) node.Parent.Left = temp_right;
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
            while(root.Parent != null)
            {
                root = root.Parent;
            }
        }
    }
}
