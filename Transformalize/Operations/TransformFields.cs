/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Linq;
using Transformalize.Main;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations
{
    public class TransformFields : AbstractOperation
    {
        private readonly Field[] _fields;
        private readonly int _transformCount;

        public TransformFields(IFields fields)
        {
            _fields = fields.ToEnumerable().OrderBy(f => f.Index).ToArray();
            _transformCount = _fields.Any() ? _fields.Sum(f => f.Transforms.Count) : 0;
            UseTransaction = false;
        }

        public TransformFields(params IFields[] fields)
        {
            var temp = new List<Field>();
            foreach (IFields f in fields)
            {
                temp.AddRange(f.ToEnumerable());
            }
            _fields = temp.OrderBy(f => f.Index).ToArray();

            _transformCount = _fields.Any() ? _fields.Sum(f => f.Transforms.Count) : 0;
            UseTransaction = false;
        }

        public TransformFields(Field field)
        {
            _fields = new[] {field};
            _transformCount = _fields.Any() ? _fields.Sum(f => f.Transforms.Count) : 0;
            UseTransaction = false;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                if (_transformCount > 0)
                {
                    foreach (Field field in _fields)
                    {
                        field.Transform(row);
                    }
                }

                yield return row;
            }
        }
    }
}