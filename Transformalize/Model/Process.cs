using System.Collections.Generic;
using System.Linq;
using Transformalize.Rhino.Etl.Core;

namespace Transformalize.Model {

    public class Process {
        private Dictionary<string, IField> _fields;

        public string Name;
        public Dictionary<string, Connection> Connections = new Dictionary<string, Connection>();
        public string Output;
        public string Time;
        public Connection OutputConnection;
        public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();
        public List<Join> Joins = new List<Join>();
        public Dictionary<string, HashSet<object>> KeyRegister = new Dictionary<string, HashSet<object>>();

        public Dictionary<string, IField> Fields {
            get {
                if (_fields == null) {
                    _fields = new Dictionary<string, IField>();
                    foreach (var entityKey in Entities.Keys) {
                        var entity = Entities[entityKey];
                        foreach (var fieldKey in entity.All.Keys) {
                            _fields[fieldKey] = entity.All[fieldKey];
                        }
                    }
                }
                return _fields;
            }
        }

        public string TruncateOutputSql() {
            return SqlTemplates.TruncateTable(Output);
        }

        public string DropOutputSql() {
            return SqlTemplates.DropTable(Output);
        }

        public string CreateOutputSql() {

            var writer = new FieldSqlWriter(Fields);
            var primaryKey = writer.FieldType(FieldType.MasterKey).Alias().Asc().Values();
            var defs = writer.Reload().ExpandXml().AddSystemFields().Output().Alias().DataType().AppendIf(" NOT NULL", FieldType.MasterKey).Values(flush: false);

            return SqlTemplates.CreateTable(this.Output, defs, primaryKey, ignoreDups: true);
        }

        public bool HasRegisteredKey(string key) {
            return KeyRegister.ContainsKey(key);
        }

        public IEnumerable<Row> RelatedRows(string foreignKey) {
            return KeyRegister[foreignKey].Select(o => new Row { { foreignKey, o } });
        }
    }
}