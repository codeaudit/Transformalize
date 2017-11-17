#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    public class FormatTransform : BaseTransform {

        private BetterFormat _betterFormat;

        public FormatTransform(IContext context = null) : base(context, "string") {
            IsMissingContext();
        }

        public override IRow Operate(IRow row) {
            row[Context.Field] = _betterFormat.Format(row);
            Increment();
            return row;
        }

        /// <summary>
        /// format initializes at this point because templates are not loaded when transforms register
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        public override IEnumerable<IRow> Operate(IEnumerable<IRow> rows) {
            var fileBasedTemplate = Context.Process.Templates.FirstOrDefault(t => t.Name == Context.Operation.Format);
            if (fileBasedTemplate != null) {
                Context.Operation.Format = fileBasedTemplate.Content;
            }

            _betterFormat = new BetterFormat(Context, Context.Operation.Format, MultipleInput);
            Run = _betterFormat.Valid;

            return base.Operate(rows);
        }

        public new IEnumerable<OperationSignature> GetSignatures() {
            yield return new OperationSignature("format") {
                Parameters = new List<OperationParameter> {
                    new OperationParameter("format")
                }
            };
        }

    }
}