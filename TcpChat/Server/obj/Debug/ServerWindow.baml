   M S B A M L   `   `   `  ����A  =Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:Server.ConvertersServer.Converters  5   	   F BCommonUtils, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullR8clr-namespace:CommonUtils.Behaviors;assembly=CommonUtilsCommonUtils.Behaviors 5   	   5clr-namespace:Server.ViewModelsServer.ViewModels  5	   	       Server.ServerWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6
convertersclr-namespace:Server.Converters     5   	   _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a=msbehaviors+http://schemas.microsoft.com/xaml/behaviors  5   	   I
behaviours8clr-namespace:CommonUtils.Behaviors;assembly=CommonUtils  5   	   0
viewModelsclr-namespace:Server.ViewModels  5	   	     � Title$  ChatClientView��5   	   $	��450��6    $	��600��6-    � DataContext 5          Server.ViewModels.ServerVm  5   
    � 	Resources 5       �� 
TargetType �� Property �� Value1   +Server.Converters.NetworkInterfaceConverter   NetworkInterfaceConverter��%  '��       '�� }     '�� �     '��     '�� R    &	  �    �� 5   
   " ��6   .�5      ��  � ��$ 3q�6'   �� 5       �@��TextAlignment$ Center=�6.   5      �� 5   
   " ��6   .�5      ��  � ��$ 3q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 3q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 3q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 3q�6'   �� 5        �@��TextAlignment$ Center=�6.   5!        5#   
   6/   5$      .��5&      �  � RowDefinitions 5'   
   �� 5(      $
��Auto �6   �� 5)      $��* �6   �� 5*      $
��Auto �6    � ColumnDefinitions 5,   
   �� 5-      $��* �6   �� 5.      $	��150 �6   .+�51   
   �� $��2��6   .+�52      A�  A� Columns$ 3��6   .+�53      �� $��IP��6   �� 54      $
��Port��6   55      A� 57      $ 3��6   .+�58      �� ��6   �� *NetworkInterfaces+	 �� SelectedItem	 6E   �� *SelectedInterface+��59      �� 5:      .R�5;      �� ��6(   �� 
 �� 	Converter#
 [  5<      5>      �� 5?      ��6   �� *Port+ �� UpdateSourceTrigger$ PropertyChanged=��� 5@      $��Start server��6@   ��6   �� *StartServerCommand+6@   5A      5B      �� 5C   
   $��1��6   $��0��6    �� 
IsReadOnly .  �� TextAlignment$
 Left=�6o    �� TextWrapping$
 Wrap=�6�    �� AcceptsReturn . " �� VerticalScrollBarVisibility$
 Auto=�5D   '   ��5C   /   �� *ServerMessages+ �� Mode$ OneWay=�.��5D   J   *  $Microsoft.Xaml.Behaviors.Interaction   	Behaviors 5E      5  /CommonUtils.Behaviors.TextBoxScrollDownBehavoir  5F      .��5G   1   5H      �� 5I   
   $��1��6   $��1��6   $
��Left=�60   $ �� IsSynchronizedWithCurrentItem$
 TrueY�5K      ��5J      �� *ConnectedClients+$ PropertyChanged=���5L      �� 5M      .R�5N      �� ��6    ��  �� Path$ UserName�5O      5Q      5R      5S      