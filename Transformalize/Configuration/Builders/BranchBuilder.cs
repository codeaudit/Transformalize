namespace Transformalize.Configuration.Builders {

    public class BranchBuilder {

        private readonly TransformBuilder _transformBuilder;
        private readonly BranchConfigurationElement _branch;

        public BranchBuilder(TransformBuilder transformBuilder, BranchConfigurationElement branch) {
            _transformBuilder = transformBuilder;
            _branch = branch;
        }

        public BranchBuilder RunIf(string field, string @operator, string value) {
            _branch.RunField = field;
            _branch.RunOperator = @operator;
            _branch.RunValue = value;
            return this;
        }

        public BranchBuilder RunIf(string field, object value) {
            _branch.RunField = field;
            _branch.RunValue = value.ToString();
            return this;
        }

        public BranchBuilder RunIf(object value) {
            _branch.RunValue = value.ToString();
            return this;
        }

        public BranchBuilder Branch(string name) {
            return _transformBuilder.Branch(name);
        }

        public ProcessConfigurationElement Process() {
            return _transformBuilder.Process();
        }

        public TransformBuilder End() {
            return _transformBuilder;
        }

        public TransformBuilder Transform(string method) {
            var transform = new TransformConfigurationElement() { Method = method };
            _branch.Transforms.Add(transform);
            return new TransformBuilder(this, transform);
        }

        public FieldBuilder Field(string name) {
            return _transformBuilder.Field(name);
        }

        public EntityBuilder Entity(string name) {
            return _transformBuilder.Entity(name);
        }

        public FieldBuilder CalculatedField(string name) {
            return _transformBuilder.CalculatedField(name);
        }

        public RelationshipBuilder Relationship() {
            return _transformBuilder.Relationship();
        }
    }
}