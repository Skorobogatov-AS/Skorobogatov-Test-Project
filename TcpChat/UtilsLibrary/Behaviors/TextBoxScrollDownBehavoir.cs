using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace TcpChat.Behaviors
{
    /// <summary>
    /// Поведение для TextBox.
    /// Производит автоматическую прокрутку вниз при добавлении текста.
    /// </summary>
    public class TextBoxScrollDownBehavoir : Behavior<TextBox>
    {
        /// <summary>
        /// Подписывает TextBox на событие.
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.TextChanged += TextBoxBase_OnTextChanged;
        }

        /// <summary>
        /// Отписывает TextBox от события.
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.TextChanged -= TextBoxBase_OnTextChanged;
        }

        /// <summary>
        /// Прокрутка вниз при добавлении текста.
        /// </summary>
        /// <param name="sender"> Textbox. </param>
        /// <param name="e"> Args. </param>
        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.IsVisible)
                textBox.ScrollToEnd();
        }
    }
}
