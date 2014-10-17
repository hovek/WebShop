(function ($) {
    $.fn.DecimalInput = function () {
        return this.each(function () {
            $(this).keydown(function (e) {
                var key = e.charCode || e.keyCode || 0;
                var decimalChar;
                if (key == 188 || key == 110) decimalChar = ',';
                else if (key == 190) decimalChar = '.';

                if (decimalChar == ',' || decimalChar == '.') {
                    if (this.value.indexOf(',') == -1 && this.value.indexOf('.') == -1) return true;

                    var selStart = this.selectionStart;
                    var staticText = this.value.replace('.', ',');
                    staticText = staticText.substring(0, this.selectionStart) + staticText.substring(this.selectionEnd, this.length);

                    var text = '';

                    for (var i = 0; i < staticText.length; i++) {
                        var l = staticText[i];

                        if (l == ',') {
                            if (i < selStart) selStart--;
                            continue;
                        }

                        text = text + l;
                    }

                    this.value = text.substring(0, selStart) + decimalChar + text.substring(selStart, text.length);
                    this.setSelectionRange(selStart + 1, selStart + 1);

                    return false;
                }

                return (
                    key == 8 || //backspace
                    key == 9 || //tab
                    key == 16 || //shift
                    key == 33 || //page up
                    key == 34 || //page down
                    key == 35 || //end
                    key == 36 || //home
                    key == 46 || //delete
                    key == 18 || //alt
                    (key >= 37 && key <= 40) || //arrows
                    (key >= 48 && key <= 57) || //numbers
                    (key >= 96 && key <= 105) //numpad numbers
                   || key == 109 || key == 189 //minus
                   || (e.ctrlKey && key == 67) //ctrl+c
                   || (e.ctrlKey && key == 86) //ctrl+v
                   || (e.ctrlKey && key == 65) //ctrl+a
                   || (e.ctrlKey && key == 88) //ctrl+x
                    );
            });

            this.addEventListener('input',
                function (e) {
                    var text = this.value;
                    var lastDecimalIndex = Math.max(text.lastIndexOf(','), text.lastIndexOf('.'));
                    var newText = '';

                    for (var i = 0; i < text.length; i++) {
                        var c = text[i];

                        if ((i != lastDecimalIndex && isNaN(c) && c != '-')
                            || c == ' ') {
                            continue;
                        }

                        newText = newText + c;
                    }

                    if (newText != text) this.value = newText;
                },
                true
            );

            this.addEventListener('blur',
              function (e) {
                  var text = this.value;

                  var lastIndexOfMinus = text.lastIndexOf('-');
                  if (lastIndexOfMinus > 0) {
                      this.value = text.substring(lastIndexOfMinus, text.length);
                  }

                  text = this.value.replace(',', '.');

                  if (text.substring(0, 1) == '.')
                      this.value = "0" + this.value;
                  else if (text.substring(text.length - 1, text.length) == '.')
                      this.value = text.replace('.', '');

              },
              true);
        });
    };

    $.fn.NumberInput = function () {
        return this.each(function () {
            $(this).keydown(function (e) {
                var key = e.charCode || e.keyCode || 0;

                return (
                    key == 8 || //backspace
                    key == 9 || //tab
                    key == 16 || //shift
                    key == 33 || //page up
                    key == 34 || //page down
                    key == 35 || //end
                    key == 36 || //home
                    key == 46 || //delete
                    key == 18 || //alt
                    (key >= 37 && key <= 40) || //arrows
                    (key >= 48 && key <= 57) || //numbers
                    (key >= 96 && key <= 105) //numpad numbers
                   || key == 109 || key == 189 //minus
                   || (e.ctrlKey && key == 67) //ctrl+c
                   || (e.ctrlKey && key == 86) //ctrl+v
                   || (e.ctrlKey && key == 65) //ctrl+a
                   || (e.ctrlKey && key == 88) //ctrl+x
                    );
            });

            this.addEventListener('input',
                function (e) {
                    var text = this.value;
                    var newText = '';

                    for (var i = 0; i < text.length; i++) {
                        var c = text[i];

                        if ((isNaN(c) && c != '-')
                            || c == ' ') {
                            continue;
                        }

                        newText = newText + c;
                    }

                    if (newText != text) this.value = newText;
                },
                true
            );

            this.addEventListener('blur',
              function (e) {
                  var text = this.value;

                  var lastIndexOfMinus = text.lastIndexOf('-');
                  if (lastIndexOfMinus > 0) {
                      this.value = text.substring(lastIndexOfMinus, text.length);
                  }
              },
              true);
        });
    };
})(jQuery);