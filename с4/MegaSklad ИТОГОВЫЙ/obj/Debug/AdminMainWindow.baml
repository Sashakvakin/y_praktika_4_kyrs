   M S B A M L   `   `   `  ����D  @MegaSklad, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:MegaSklad	MegaSklad  5   	       MegaSklad.AdminMainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:MegaSklad     5   	     � Title$+  %MegaSklad: Администратор��5   	   $	��600��6*   $
��1000��67    � WindowStartupLocation$ CenterScreen=�6D    � Icon$ 	/icon.png��6i    � 	Resources 5
      *   $MegaSklad.MultiImageConverterProfile   MultiImageConverterProfile��%   &	          5   
   6+   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $
��Auto �6   $	��120��6,   �� 5      $��* �6   .+�5   
   �� $��0��6   
�������$	��250��6<   .+�5      �� -   5        $��Профиль��5      $��	-1 5 -1 3q�6'   
 � Tag$ Профиль��6S   �� 5      -   5        $��Склады��5      $��	-1 5 -1 3q�6&   $ Склады��6R   �� 5      -   5        $��Товары��5      $��	-1 0 -1 3q�6&   $ Товары��6R   �� 5      -   5        $��Клиенты��5      $��	-1 0 -1 3q�6'   $ Клиенты��6S   �� 5      -   5        $��Поставщики��5      $��	-1 0 -1 3q�6*   $ Поставщики��6V   �� 5      -   5        $��Пользователи��5      $��	-1 0 -1 3q�6,   $ Пользователи��6X   5      � 5   
   $��1��6   
������� � RowDefinitions 5      �� 5      $
��Auto �6    �� 5       $��* �6    �� 5!      $
��Auto �6    .+�5$      �� -   5         ��Name$ ProfilePanel��5$      $y�	Collapsed=�6/   $��0��6F   $��10q�6S   .+�5%      �� $1��+Профиль Администратора��6   $��18(�6:   $
��Bold"�6H   $��0,0,0,10q�6Z   �� 5&      -   5         ��Name$ ProfileImage��5&      $	��100��6.   $	��100��6:   $��UniformToFill=�6G   $��0,0,0,10q�6_   ��6q   �� *ProfileImageSource+�� 5'      -	   5        	 ��Name$	 NameTextBlock��5'      $��0,0,0,5q�6z   ��63   �� *-+UserProfile.имя_пользователя+
 �� StringFormat$
 Имя: {0}��6z   �� 5(      -
   5        $	 EmailTextBlock��5(      $��0,0,0,5q�6r   ��64   �� *UserProfile.email+$
 
Email: {0}��6r   �� 5)      -   5        $	 PhoneTextBlock��5)      $��0,0,0,5q�6v   ��64   �� *UserProfile.телефон+$
 Телефон: {0}��6v   �� 5*      -   5        $	 RoleTextBlock��5*      $��0,0,0,5q�6o   ��63   �� *UserProfile.роль+$
 Роль: {0}��6o   �� 5+      -   5        $/��)Редактировать профиль��5+      $��Center=�6Y   $��10q�6v   �� 5,      -   5        $&�� Выйти из аккаунта��5,      $��Center=�6P   $��10q�6m   
���  ��
�������6�   5-      &   System.Windows.Controls.DataGrid  5/      -   5          Name$ ContentDataGrid��5/         AutoGenerateColumns .     
IsReadOnly . $��1��6^   
�������6k     50      -   5        $ UsersDataGrid��50       .   . $y�	Collapsed=�6\   $��1��6s   
�������6�   �� 51      -   5        $ DataGridButtonsPanel��51       �� Orientation$ 
Horizontal=�67   $��Center=�6P   $��2��6m   $��10q�6z   .+�52      �� -   5        $��Добавить��52      $��5q�6,   67   �� 53      -   5        $ ��Редактировать��53      $��5q�61   6<   �� 54      -   5        $��Удалить��54      $��5q�6+   66   55      �� 56      -   5        $ UsersDataGridButtonsPanel��56      $y�	Collapsed=�6<   $ 
Horizontal=�6S   $��Center=�6l   $��2��6�   $��10q�6�   .+�57      �� -   5        $��Добавить��57      $��5q�6,   67   �� 58      -   5        $ ��Редактировать��58      $��5q�61   6<   �� 59      -   5        $��Удалить��59      $��5q�6+   66   5:      5;      5<      5=      