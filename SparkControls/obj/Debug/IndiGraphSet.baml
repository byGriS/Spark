   M S B A M L   `   `   `  ����H  DSparkControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SparkControlsSparkControls  5   	        SparkControls.IndiGraphSet   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:SparkControls     5   	   -   5          � Title$F  @Настройки графического индикатора��5   	   $	��275��5	   	   $	��290��6    � 
ResizeMode$ NoResize=�5
   	    � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6   .+�5   
   �� -   5         ��Name$ 	spHistory��5       �� Orientation$ 
Horizontal=�6(   $��5q�6A   .+�5      �� $D��>Отображать кол-во последних точек��6   �� 5      -   5         ��Name$ 	tbHistory��5      $	��255��6)   $��30��64   $��5 2 2 2q�6?   $��Center=�6P   $��Center=�5      5      5      �� 5   
   $��1��6   .��6   #!Список параметров61   �� 5   
   -   5         ��Name$ 
listParams��5      $��2��6   $��Visible=�63    �� SelectionMode$ Multiple=�6f   ��5      �� 5      .R�5      �� ��6   �� *.Title+	 d� 	IsChecked	 6:   �� 
 �� RelativeSource
 ��  �� AncestorType" �� �� Path$ 
IsSelected�5       5"      �� 5#   
   $��3��6   $��2��6"   .+�5$      �� -   5        $��Применить��5$      $��Center=�5%      $��5,5,5,5q�5&      $��	10 2 10 2q�5'      5(      5)      5*      5,      