   M S B A M L   `   `   `  ����H  DSparkControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:SparkControlsSparkControls  5   	   #    SparkControls.SelectIndicator   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:SparkControls     5   	     � Title$  Индикаторы��5   	   $	��450��6   $	��450��6)    � WindowStartupLocation$ CenterScreen=�5	   	   .��5
      �  � RowDefinitions 5      �� 5      �� 5      $��32 �6   .+�5      � -   5         �Name$ listIndicators��5       5      �� 5   
   �� 5   
   �� 5   
    � ColumnDefinitions 5      �� 5   
   �� 5   
   �� 5   
   .+�5         SparkControls.IndiDigital     SparkControls.IndiDigital   	MouseDown$ Indi_MouseDown��6o   $��0��6   $��0��6'   $��10q�67   
�������$��3q�6[   6o      SparkControls.IndiBoolean  5         SparkControls.IndiBoolean   	MouseDown$ Indi_MouseDown��6o   $��0��6   $��1��6'   $��10q�67   
�������$��3q�6[   6o      SparkControls.IndiColumn  5         SparkControls.IndiColumn   	MouseDown$ Indi_MouseDown��6n   $��0��6   $��2��6&   $��10q�66   
�������$��3q�6Z   6n      SparkControls.IndiArrow  5         SparkControls.IndiArrow   	MouseDown$ Indi_MouseDown��6m   $��1��6   $��0��6%   $��10q�65   
�������$��3q�6Y   6m   !	   SparkControls.IndiManyParam	  5      !
   SparkControls.IndiManyParam	 
  	MouseDown$	 Indi_MouseDown��6q   $��1��6   $��1��6)   $��10q�69   
�������$��3q�6]   6q      SparkControls.IndiGraph  5          SparkControls.IndiGraph
   	MouseDown$
 Indi_MouseDown��6m   $��1��6   $��2��6%   $��10q�65   
�������$��3q�6Y   6m       SparkControls.IndiSendSoOn  5!          SparkControls.IndiSendSoOn   	MouseDown$ Indi_MouseDown��6p   $��2��6   $��0��6(   $��10q�68   
�������$��3q�6\   6p   !   SparkControls.IndiSendStart  5"      !   SparkControls.IndiSendStart   	MouseDown$ Indi_MouseDown��6q   $��2��6   $��1��6)   $��10q�69   
�������$��3q�6]   6q   #   SparkControls.IndiInputStatus  5#      #   SparkControls.IndiInputStatus   	MouseDown$ Indi_MouseDown��6s   $��2��6   $��2��6+   $��10q�6;   
�������$��3q�6_   6s   5%      �� 5'      $��3��6   
������� �� Orientation$ 
Horizontal=�61   $��RightToLeft=�6J   .+�5(      �� -   5        $��Закрыть��5(      $��3q�6!   $��3q�6B   $��75��6M   �� 5)      -   5         ��Name$ btnOk��5)      $��Выбрать��6   $��3q�60   $��3q�6M   $��75��6X   {�.  6c   5*      5+      5,      