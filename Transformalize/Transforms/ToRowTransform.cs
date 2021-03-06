using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms.System;

namespace Transformalize.Transforms {
    public class ToRowTransform : BaseTransform {

        private readonly IRowFactory _rowFactory;
        private readonly Field _input;
        private readonly Field[] _fields;
        private SetSystemFields _setSystemFields;
        private readonly Field _hashCode;
        private Field[] _fieldsToHash;

        public ToRowTransform(IContext context = null, IRowFactory rowFactory = null) : base(context, null) {

            if (IsMissingContext()) {
                return;
            }

            if (LastMethodIsNot("split", "sort", "reverse")) {
                return;
            }

            if (rowFactory == null) {
                Run = false;
                Context.Error("The toRow() method did not receive a row factory.");
                return;
            }

            ProducesRows = true;

            _rowFactory = rowFactory;
            _fields = Context.Entity.GetAllFields().ToArray();
            _input = SingleInput();

            // this bit can be encapsulated ,it is always needed for producing rows (it's in FromXml too)
            _setSystemFields = new SetSystemFields(context);
            _hashCode = Context.Entity.TflHashCode();
            _fieldsToHash = _fields.Where(f => !f.System).ToArray();
        }

        public override IRow Operate(IRow row) {
            throw new NotImplementedException();
        }

        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            foreach (var outer in rows) {
                var values = (string[])outer[_input];
                if (values.Length == 0) {
                    yield return outer;
                } else {
                    foreach (var value in values) {
                        var inner = _rowFactory.Clone(outer, _fields);
                        inner[Context.Field] = value;

                        // this has to be done whenever adding rows
                        if (!Context.Process.ReadOnly) {
                            inner = _setSystemFields.Operate(inner);
                            inner[_hashCode] = HashcodeTransform.GetHashCode(_fieldsToHash.Select(f => inner[f]));
                        }

                        yield return inner;
                    }
                }

            }
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("torow");
            yield return new OperationSignature("torows");
        }
    }
}