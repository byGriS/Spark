   M S B A M L   `   `   `  ����A  =Spark, Version=1.5.35.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SparkSpark  5          Spark.ParamsTextOutput   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      %localclr-namespace:Spark     5        � 	Resources  5	         add 
 insert 
 remove��%�   &	        &	 +     &	 Y     �� 5
   
   $��Resources/add.png��6   �� 5   
   $��Resources/insert.png��6   �� 5   
   $��Resources/remove.png��6   5      .��5      �  � RowDefinitions 5   
   �� 5      $��30 �6   �� 5      .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   .+�5      �� -   5        #��[  5      $��3q�6K   $��0q�6V   
������ 6j   �� 5      -   5         ��Name$ bInsertParam��5      #��[ 6+   $��3q�6g   $��0q�6r   
������ $y�Hidden=�6�   �� 5      -   5        $ bRemoveParam��5      #��[ 6+   $��3q�6g   $��0q�6r   
������ $y�Hidden=�6�   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ ParamsSerialTextOut��5      $��1��5         AutoGenerateColumns .     CanUserDeleteRows .     CanUserAddRows .     SelectionMode$ Extended=�5      	   SelectionUnit$	 CellOrRowHeader=�5      
   CanUserSortColumns
 .     RowHeaderWidth$ 20Q�5!      ��5      �� *.+   Columns 5#      0  *System.Windows.Controls.DataGridTextColumn  5$      ,  &System.Windows.Controls.DataGridColumn   
IsReadOnly .    Header$ Название��5&         Width5  /System.Windows.Controls.DataGridLengthConverter$ 4* 5'      1  +System.Windows.Controls.DataGridBoundColumn   Binding 5(      ��  �� Path$ Title�5*      5+      5,      