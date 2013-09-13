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

using System.Data;
using System.Linq;
using Transformalize.Main;
using Transformalize.Libs.Rhino.Etl;
using Transformalize.Libs.Rhino.Etl.Operations;

namespace Transformalize.Operations
{
    public class EntityInputKeysExtractDelta : InputCommandOperation
    {
        private readonly Entity _entity;
        private readonly string[] _fields;

        public EntityInputKeysExtractDelta(Entity entity)
            : base(entity.InputConnection)
        {
            _entity = entity;
            _fields = _entity.PrimaryKey.ToEnumerable().Select(f => f.Alias).ToArray();

            _entity.CheckDelta();

            if (!_entity.HasRows)
            {
                Debug("No data detected in {0}.", _entity.Alias);
            }

            if (!_entity.HasRange) return;

            if (_entity.BeginAndEndAreEqual())
            {
                Debug("No changes detected in {0}.", _entity.Alias);
            }
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            var row = new Row();
            foreach (string field in _fields)
            {
                row[field] = reader[field];
            }
            return row;
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandTimeout = 0;
            cmd.CommandText = _entity.HasRange ? _entity.KeysRangeQuery() : _entity.KeysQuery();
            cmd.CommandType = CommandType.Text;

            if (_entity.HasRange)
                AddParameter(cmd, "@Begin", _entity.Begin);
            AddParameter(cmd, "@End", _entity.End);
        }

        public bool NeedsToRun()
        {
            return _entity.NeedsUpdate();
        }
    }
}