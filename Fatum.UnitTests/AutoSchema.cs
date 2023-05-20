using Proliferation.Fatum;
using System.ComponentModel.Design;

namespace Fatum.UnitTests
{
    public class AutoSchema
    {
        [Fact]
        public void AutoSchema_Generate_Simple_Test()
        {
            //  Arrange

            Tree tree = new Tree();
            tree.AddNode(new Tree()
            {
                Value = "123"
            }, "first");
            tree.AddNode(new Tree()
            {
                Value = "456"
            }, "second");
            tree.AddNode(new Tree()
            {
                Value = "789"
            }, "third");

            //  Act

            Tree? schema = Proliferation.Fatum.AutoSchema.Generate(tree);

            //  Assert

            Assert.NotNull(schema);
            Assert.True(string.Compare(schema.tree[0].GetElement("FieldName"), "first")==0);
            Assert.True(string.Compare(schema.tree[1].GetElement("FieldName"), "second") == 0);
            Assert.True(string.Compare(schema.tree[2].GetElement("FieldName"), "third") == 0);
        }
    }
}