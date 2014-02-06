// Npgsql.NpgsqlQueryV3.cs
//
// Author:
//     Dave Joyner <d4ljoyn@yahoo.com>
//
//    Copyright (C) 2002 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.

using System.IO;

namespace Transformalize.Libs.Npgsql
{
    /// <summary>
    /// Summary description for NpgsqlQuery
    /// </summary>
    internal sealed class NpgsqlQueryV3 : NpgsqlQuery
    {
        public NpgsqlQueryV3(byte[] command)
        : base(command)
        {
        }

        public NpgsqlQueryV3(string command)
        : base(command)
        {
        }

        public override void WriteToStream(Stream outputStream)
        {
            if (NpgsqlEventLog.Level >= LogLevel.Debug)
            {
                // Log the string being sent.
                PGUtil.LogStringWritten(BackendEncoding.UTF8Encoding.GetString(commandText));
            }

            // Send the query to server.
            // Write the byte 'Q' to identify a query message.
            outputStream.WriteByte((byte)FrontEndMessageCode.Query);

            // Write message length. Int32 + string length + null terminator.
            PGUtil.WriteInt32(outputStream, 4 + commandText.Length + 1);

            outputStream.WriteBytesNullTerminated(commandText);
        }
    }
}
