﻿
/*Skripta za Skirvanje kontrola "x" */
function CustomScripts_HideControl(id)
{
    document.getElementById(id).style.display='none';
}
/*datetime picker*/
jQuery(function ($) {
        $.datepicker.regional['hr'] = {
            closeText: 'Zatvori',
            prevText: '&#x3c;',
            nextText: '&#x3e;',
            currentText: 'Danas',
            monthNames: ['Siječanj', 'Veljača', 'Ožujak', 'Travanj', 'Svibanj', 'Lipanj',
            'Srpanj', 'Kolovoz', 'Rujan', 'Listopad', 'Studeni', 'Prosinac'],
            monthNamesShort: ['Sij', 'Velj', 'Ožu', 'Tra', 'Svi', 'Lip',
            'Srp', 'Kol', 'Ruj', 'Lis', 'Stu', 'Pro'],
            dayNames: ['Nedjelja', 'Ponedjeljak', 'Utorak', 'Srijeda', 'Četvrtak', 'Petak', 'Subota'],
            dayNamesShort: ['Ned', 'Pon', 'Uto', 'Sri', 'Čet', 'Pet', 'Sub'],
            dayNamesMin: ['Ne', 'Po', 'Ut', 'Sr', 'Če', 'Pe', 'Su'],
            weekHeader: 'Tje',
            dateFormat: 'dd.mm.yy.',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: '',
            yearRange: "1920:2012"
        };
        $.datepicker.setDefaults($.datepicker.regional['hr']);
});
$(function () {
    $("#datepicker").datepicker({
        changeMonth: true,
        changeYear: true
    });
});
