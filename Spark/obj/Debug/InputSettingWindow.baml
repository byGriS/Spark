   M S B A M L   `   `   `  ����A  =Spark, Version=1.5.35.0, Culture=neutral, PublicKeyToken=nullclr-namespace:SparkSpark  5   	       Spark.InputSettingWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:Spark     5   	   -   5          � Title$%  Настройка входов��5   	    � WindowStartupLocation$ CenterScreen=�5	   	    � ShowInTaskbar .  $	��600��5   	   $	��950��5   	    � 	Resources 5         addInput  insertInput  removeInput��%�   &	        &	 1     &	 d     �� 5   
   $��Resources/SerailAdd.png��6!   �� 5   
   $��Resources/EthernetAdd.png��6$   �� 5   
   $��Resources/InputRemove.png��6$   5      .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $��32 �6   .+�5   
   �  � ColumnDefinitions 5      �� 5      $	��100 �6#   �� 5      $	��260 �6#   �� 5      $��* �6#   .+�5      �  5       �� 5!      $��26 �6$   �� 5"      $��26 �6$   �� 5#      $��* �6$   .+�5%      ��  �� Orientation$ 
Horizontal=�6   .+�5&      �� -   5        #��[  5&      $��3q�6Y   $��0q�6d   
������ 6x   �� 5'      -   5        #��[ 5'      $��3q�6^   $��0q�6i   
������ 6}   �� 5(      -   5         ��Name$ bRemoveInput��5(      #��[ 63   $��3q�6t   $��0q�6   
������ $y�Hidden=�6�   5)      �� 5*      $��1��6   $��Список входов��6%   �� 5+      -   5         ��Name$ 	listInput��5+      $��2��5,      $��Stretch=�5.      ��5/      �� 50      .R�51      �� ��6(   �� *Title+52      54      55          Spark.SerialSettingControl  57      -   5        	  Name$	 serialSetting��57   )   $��1��6@   $y�Hidden=�6P   "   Spark.EthernetSettingControl  58      -   5        
  Name$
 ethernetSetting��58   +   $��1��6D   $y�Hidden=�6T      Spark.ParamsTextInput  5:      -   5          Name$ 
paramsText��5:   $   $��2��68   $y�Hidden=�6H      Spark.ParamsModbusInput  5;      -	   5          Name$ paramsModbus��5;   &   $��2��6<   $y�Hidden=�6L   5=      �� 5?   
   $��1��6   
�������$ 
Horizontal=�65   $��RightToLeft=�6N   .+�5@      �� -
   5        $
��Ок��5@      $��3q�6"   $��3q�6?   $��75��6J   �� 5A      -   5        $��Отменить��5A      $��3q�6(   $��3q�6I   $��75��6T   5B      5E      5F      