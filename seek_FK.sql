SELECT detail_table.column_name,
	   ref_Table.TABLE_NAME,
	   ref_Table.column_name
  FROM all_constraints  constraint_info,
       all_cons_columns detail_table,
       all_cons_columns ref_Table
 WHERE constraint_info.constraint_name = detail_table.constraint_name
   AND constraint_info.r_constraint_name = ref_Table.constraint_name
   AND detail_table.POSITION = ref_Table.POSITION
   AND constraint_info.constraint_type = 'R'
   AND detail_table.TABLE_NAME = 'KLIENCI';
