   M S B A M L   `   `   `  ����F  BCommonUtils, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullR8clr-namespace:CommonUtils.Behaviors;assembly=CommonUtilsCommonUtils.Behaviors  5   	   B >TcpChat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:TcpChat.ViewModelsTcpChat.ViewModels 5   	      TcpChat.ChatClientView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   _ [Microsoft.Xaml.Behaviors, Version=1.1.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a=msbehaviors+http://schemas.microsoft.com/xaml/behaviors  5   	   I
behaviours8clr-namespace:CommonUtils.Behaviors;assembly=CommonUtils  5   	   -models clr-namespace:TcpChat.ViewModels  5   	     � Title$  ChatClientVmView��5
   	   $	��450��6"   $	��600��6/    � DataContext 5      %  TcpChat.ViewModels.ChatClientVm  5   
   *  $Microsoft.Xaml.Behaviors.Interaction   Triggers 5      +  %Microsoft.Xaml.Behaviors.EventTrigger  5   
      	EventName$ Closing��6#   *  $Microsoft.Xaml.Behaviors.TriggerBase   Actions. 5      2  ,Microsoft.Xaml.Behaviors.InvokeCommandAction     Command 6.   �� *DisconnectCommand+5       � 	Resources 5       �� 
TargetType �� Property	 �� Value��%�  '��       '�� }     '�� �     '��     �� 5   
   " ��6   .�5      ��  � ��$	 3q�6'   �� 5       �@��TextAlignment$	 Center=�6.   5      �� 5   
   " ��6   .�5      ��  � ��$	 3q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$	 3q�6'   5      �� 5    
   " ��6   .�5!      ��  � ��$	 3q�6'   �� 5"       �@��TextAlignment$	 Center=�6.   5#      5$      .��5&      � 
 � RowDefinitions
 5'   
   �� 5(      $
��Auto �6   �� 5)      $��* �6   �� 5*      $
��Auto �6    � ColumnDefinitions 5,   
   �� 5-      $��* �6   �� 5.      $	��150 �6   .+�51   
   �� $��2��6   .+�52      A�  A� Columns$ 4��6   .+�53      �� $��IP��6   �� 54      $
��Port��6   �� 55      $��UserName��6   56      A� 58      $ 4��6   .+�59      �� ��6   �� *Ip+ �� UpdateSourceTrigger$ PropertyChanged=��� 5:      ��6   �� *Port+$ PropertyChanged=��� 5;      ��6   �� *
UserName+$ PropertyChanged=��� 5<      $��Connect��6<   ��6   �� *ConnectCommand+6<   5=      5>      �� 5@   
   $��1��6   $��1��6   $
��Left=�60   $ �� IsSynchronizedWithCurrentItem$
 TrueY�5B      ��5A      �� *	ChatUsers+$ PropertyChanged=���5C      �� 5D      .R�5E      �� ��6    ��  �� Path$ UserName� J� InputBindings 5F      d� 5G       d� MouseAction$ LeftDoubleClicke�6+    �� Command 5H   +   �� $" DataContext.StartChatCommand� �� RelativeSource ��  �� AncestorType" � �� CommandParameter 5I   +   �� $ Id�5K      5L      5N      �� 5P   
   $��2��6   $��0��6    �� TextAlignment$
 Left=�6/    �� TextWrapping$
 Wrap=�6D   " �� VerticalScrollBarVisibility$
 Auto=�6X   $	��200��6{   ��5Q      �� *	Message+$ PropertyChanged=�.��6P    5R      �� 5S      
 �� Key$ Enter��6    6)   �� *SendMessageCommand+.��5T   %   5U      �� 5W   
   $��2��6   $��1��6   $
��Send��6/   $��20��6>   $��Bottom=�5X   8   ��6   �� *SendMessageCommand+68   �� 5Z   
   $��1��6   $��0��6"   $
 TrueY�5[      ��5\      �� $ ChatTabs� �� SelectedItem 6;   �� *ActiveClient+��5]      �� 5^       �� DataType$ models:ServerClient��6   .R�5_      �� ��6    �� *
UserName+ �� Mode$ OneWay=�6J   5`       �� ContentTemplate 5b      �� 5c      $ models:ServerClient��6   .R�5d      ��  �� 
IsReadOnly . $
 Left=�6T   $
 Wrap=�6i    �� AcceptsReturn . $
 Auto=�5e   3   ��5d      �� *Chat+$ OneWay=�.��5e   V       	Behaviors  5f      5   /CommonUtils.Behaviors.TextBoxScrollDownBehavoir  5g      .��5h   =   5i      5j      5l      5m      5n      