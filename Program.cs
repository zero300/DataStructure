using Data_Structure.Structure;

Console.WriteLine("Start");

AVL_Tree tree = new AVL_Tree();

tree.Insert(10);
tree.Insert(6);
tree.Insert(12);
tree.Insert(13);
tree.Insert(14);
tree.Insert(15);
tree.Insert(16);
tree.Insert(17);
tree.Insert(18);

//tree.Remove(10);
//6,12,10,14,16,18,17,15,13,
Console.WriteLine( tree.ShowTree() );