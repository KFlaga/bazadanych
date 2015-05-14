SELECT detail_table.COLUMN_NAME, SEARCH_CONDITION_VS
  FROM all_constraints  constraint_info,
       all_cons_columns detail_table
 WHERE constraint_info.constraint_name = detail_table.constraint_name
   AND constraint_info.constraint_type = 'C'
   AND detail_table.TABLE_NAME = 'KLIENCI';