$(document).ready(function () {

    $('#addBook').click(function (e) {
        location.href = "/Book/AddBook";
    });

    $('#backToList').click(function (e) {
        location.href = "/Book/List";
    });

    $('#addAuthor').click(function (e) {
        var element = '<div><label>Add Author</label><input type="text" class="author" /></div>';
        $(".container").append(element);
    });

    $('#addBookToRepository').click(function (e) {
        $('#bookTitleError').html("");
        $('#countError').html("");
        $('#authorError').html("");


        var arrayOfAuthors = $('.author').map(function () { return $(this).val(); }).get();
        var authors = [];
        for (var i = 0; i < arrayOfAuthors.length; i++) {
            authors.push({
                AuthorName: arrayOfAuthors[i],
                Books: null
            });
        }

        var book = {
            Title: $('#bookTitle').val(),
            AvailableBooks: $('#allBooks').val(),
            AllBooks: $('#allBooks').val(),
            Authors: authors
        }

        if (book.Title == "") {
            $('#bookTitleError').html("Enter book title");
            return false;
        }

        if (book.AllBooks == "" || book.AllBooks <= 0 || !validateNumber(book.AllBooks)) {
            $('#countError').html("Please, enter correct number of books");
            return false;
        }

        if (book.Authors.length == 0) {
            $('#authorError').html("Please, enter author");
            return false;
        }

        $.ajax({
            url: "/Book/AddBook",
            method: "POST",
            data: JSON.stringify(book),
            contentType: "application/json",
            success: function (redirect) {
                window.location = redirect;}
        });
    });

    $('#changeBookQuantity').click(function (e) {
        var availableBook = parseInt($('#editBookQuantity').val()) -
            parseInt($('#initiallBookChange').val()) + parseInt($('#availableBookChange').val());

        if (availableBook < 0) {
            $('#quantityError').html("Available book is less than 0. Please, enter correct quantity");
            return false;
        }

        if (!validateNumber($('#editBookQuantity').val())) {
            $('#quantityError').html("Please enter number.");
            return false;
        }

        var book = {
            BookId: $('#bookId').val(),
            Title: $('#editBookTitle').val(),
            AllBooks: $('#editBookQuantity').val(),
            AvailableBooks: availableBook,
        }

        $.ajax({
            url: "/Book/ChangeBookQuantity",
            method: "POST",
            data: JSON.stringify(book),
            contentType: "application/json",
            success: function (redirect) {
                window.location = redirect;
            }
        });
    });

    function validateNumber(number) {
        var numberReg = /^[0-9]*$/;
        return numberReg.test(number);
    }

    $('.takeBook').click(function (e) {
        var fName = $(this).attr('id');
        var bookId = 'td#bookId' + fName;
        var title = 'td#title' + fName;
        var availableBooks = 'td#availableBooks' + fName;
        var allBooks = 'td#allBook' + fName;

        var book = {
            BookId: $(bookId).text(),
            Title: $(title).text(),
            AvailableBooks: $(availableBooks).text(),
            AllBooks: $(allBooks).text(),
        }

        $.ajax({
            url: "/Book/TakeBook",
            method: "POST",
            data: JSON.stringify(book),
            contentType: "application/json",
            success: function (response) {
                if (response != null && response.success) {
                    location.href = "/Book/List";
                } else {
                    $('#takeSummaryError').html("You have already taken this book. Please, choose another.");
                }
            }
        });
    });

    $('select').change(function () {
        var optionSelected = $(this).find("option:selected");
        var valueSelected = optionSelected.val();
        var rowCount = $('#booksCount').val();

        if (valueSelected == "All books") {
            for (var i = 0; i < rowCount; i++) {
                var visibleBookCount = '#visibleBookCount' + i;
                var allBooks = '#allBook' + i;
                $(visibleBookCount).html($(allBooks).text());
            }
        }
        if (valueSelected == "Books available") {
            for (var i = 0; i < rowCount; i++) {
                var allBooks = '#allBook' + i;
                var availableBooks = '#availableBooks' + i;
                var visibleBookCount = '#visibleBookCount' + i;
                $(visibleBookCount).html($(availableBooks).text());
            }
        }
        if (valueSelected == "Books taken by the user") {
            for (var i = 0; i < rowCount; i++) {
                var allBooks = '#allBook' + i;
                var availableBooks = '#availableBooks' + i;
                var visibleBookCount = '#visibleBookCount' + i;
                var booksTakenByUser = parseInt($(allBooks).text()) -
                   parseInt($(availableBooks).text());

                $(visibleBookCount).html(booksTakenByUser);
            }
        }
    });

// sorting

    $('th').click(function () {
        var table = $(this).parents('table').eq(0)
        var rows = table.find('tr:gt(0)').toArray().sort(comparer($(this).index()))
        this.asc = !this.asc
        if (!this.asc) { rows = rows.reverse() }
        for (var i = 0; i < rows.length; i++) { table.append(rows[i]) }
    })
    function comparer(index) {
        return function (a, b) {
            var valA = getCellValue(a, index), valB = getCellValue(b, index)
            return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.localeCompare(valB)
        }
    }
    function getCellValue(row, index) { return $(row).children('td').eq(index).html() }

// paging

    $('table.paginated').each(function () {
        var currentPage = 0;
        var numPerPage = 3;
        var $table = $(this);
        $table.bind('repaginate', function () {
            $table.find('tbody tr').hide().slice(currentPage * numPerPage, (currentPage + 1) * numPerPage).show();
        });
        $table.trigger('repaginate');
        var numRows = $table.find('tbody tr').length;
        var numPages = Math.ceil(numRows / numPerPage);
        var $pager = $('<div class="pager"></div>');
        for (var page = 0; page < numPages; page++) {
            $('<span class="page-number"></span>').text(page + 1).bind('click', {
                newPage: page
            }, function (event) {
                currentPage = event.data['newPage'];
                $table.trigger('repaginate');
                $(this).addClass('active').siblings().removeClass('active');
            }).appendTo($pager).addClass('clickable');
        }
        $pager.insertBefore($table).find('span.page-number:first').addClass('active');
    });

});