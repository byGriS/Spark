   M S B A M L   `   `   `  ����A  =Spark, Version=1.5.35.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SparkSpark  5   	       Spark.UnitSetWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:Spark     5   	   -   5          � Title$8  2Настройка типов параметров��5   	   $	��300��5	   	   $	��300��5
   	    � ShowInTaskbar .   � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�5   	    � 	Resources 5         add 
 insert 
 remove  add2  insert2  remove2��%J  &	        &	 +     &	 Y     &	 �     &	 �     &	 �     �� 5   
   $��Resources/add.png��6   �� 5   
   $��Resources/insert.png��6   �� 5   
   $��Resources/remove.png��6   �� 5   
   $��Resources/add.png��6   �� 5   
   $��Resources/insert.png��6    �� 5   
   $��Resources/remove.png��6    5      .��5      �  � RowDefinitions 5   
   �� 5      $��24 �6   �� 5      �� 5      $��32 �6   .+�5   
   �� $��0��6   
������� �� Orientation$ 
Horizontal=�65   .+�5       o� $
��Left=�6   ��6.   Z� .@�5!      m� $��Файл��6   $��24��6*   .@�5"      m� -   5        $ ��Импортировать��5"      66   m� 5#      -   5        $"��Экспортировать��5#      67   5$      5%      5&      � 5*   
   $��1��6    � ColumnDefinitions 5+      �� 5,      �� 5-       5/      �� 50      $��30 �6    �� 51      .+�54      �� $��0��6   $ 
Horizontal=�6&   .+�55      �� -   5        #��[  55      $��3q�6O   $��0q�6Z   
������ 6n   �� 56      -   5         ��Name$ bInsertParam��56      #��[ 6/   $��3q�6k   $��0q�6v   
������ $y�Hidden=�6�   �� 57      -   5        $ bRemoveParam��57      #��[ 6/   $��3q�6k   $��0q�6v   
������ $y�Hidden=�6�   58      &   System.Windows.Controls.DataGrid  5:      -   5        	  Name$	 dgParam��5:      $��1��5;      
   AutoGenerateColumns
 .     CanUserDeleteRows .     CanUserAddRows .     SelectionMode$ Single=�5?         SelectionUnit$ CellOrRowHeader=�5@         RowHeaderWidth$ 20Q�5A         CanUserSortColumns .  ��5B      ��    Columns 5F      0  *System.Windows.Controls.DataGridTextColumn  5G      ,  &System.Windows.Controls.DataGridColumn   Header$ Параметр��5H         Width5  /System.Windows.Controls.DataGridLengthConverter$ 1* 5I      1  +System.Windows.Controls.DataGridBoundColumn   Binding 5J      ��  �� Path$ Title�5M      �� 5O      $��0��6   $��1��6&   $ 
Horizontal=�66   .+�5P      �� -   5        $ bAddUnit��5P      #��[ 6+   $��3q�6a   $��0q�6l   
������ $y�Hidden=�6�   �� 5Q      -	   5        $ bInsertUnit��5Q      #��[ 6.   $��3q�6j   $��0q�6u   
������ $y�Hidden=�6�   �� 5R      -
   5        $ bRemoveUnit��5R      #��[ 6.   $��3q�6j   $��0q�6u   
������ $y�Hidden=�6�   5S        5U      -   5        $	 dgUnit��5U      $��1��5V      $��1��5W      
 .   .   .  $ Single=�5[      $ CellOrRowHeader=�5\      $ 20Q�5]       .   5a        5b      $ Ед. изм.��5c      $ 1* 5d       5e      �� $ Title�5h      5i      �� 5k   
   $��2��6   
�������$ 
Horizontal=�65   $��RightToLeft=�6N   .+�5l      �� -   5        $
��Ок��5l      $��3q�6"   $��3q�6?   $��75��6J   �� 5m      -   5        $��Отменить��5m      $��3q�6(   $��3q�6I   $��75��6T   5n      5o      5p      