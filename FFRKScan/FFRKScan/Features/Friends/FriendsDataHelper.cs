using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRKScan.Features.Friends
{
    public static class FriendsDataHelper
    {
        public static void UpdateControl(this IEnumerable<DataTargetProfiles> self, AutoDataGrid grid)
        {
            if (grid == null)
            {
                return;
            }

            if (self != null)
            {
                grid.Populate(DataRelations.GetColumns(), DataRelations.GetPrimaryKey(), (dt) => self.UpdateRows(dt));
            }
            else
            {
                grid.Clear();
            }
        }

        static void UpdateRows(this IEnumerable<DataTargetProfiles> self, DataTable dt)
        {
            int i = 0;

            foreach (var item in self)
            {
                var row = item.ToRow(dt, (short)(i + 1));
                dt.Rows.Add(row);
                i += 1;
            }
        }
        public static void UpdateControl(this DataRelations self, AutoDataGrid grid)
        {
            if (grid == null)
            {
                return;
            }

            if (self != null)
            {
                grid.Populate(DataRelations.GetColumns(), DataRelations.GetPrimaryKey(), (dt) => self.UpdateRows(dt));
            }
            else
            {
                grid.Clear();
            }
        }
    }
}
