
///INSERT PRODUCT INFORMATION

function save() {
    debugger;

    var param = new Object();
    param['CategoryId'] = $("#CategoryId").val();
    param['ProductName'] = $("#ProductName").val();


    $.ajax({
        method: "POST",
        url: "/Test/InsertProduct",
        data: param,
        success: function (data) {
            debugger;
            if (data == 0) {
                alert('Failed to add the Product');
            }
            else {
                alert('Product ' + data.ProductName + ': Added Successfully')
            }
            $("#ProductName").val('');
            $("#ProductName").focus();
            //$("#CategoryId").val(-1);


        }
    });
}


////UPDATE PRODUCT

function update() {
    debugger;

    var param = new Object();
    param['ProductId'] = $("#ProductId").val();
    param['CategoryId'] = $("#CategoryId").val();
    param['ProductName'] = $("#ProductName").val();


    $.ajax({
        method: "POST",
        url: "/Test/UpdateProduct",
        data: param,
        success: function (data) {
            debugger;
            if (data == 0) {
                alert('Failed to add the Product');
            }
            else {
                alert('Product ' + data.ProductName + ': Updated Successfully')
            }
            window.location.href = "/Test/ListOfProduct";


        }
    });
}


///REDIRECT TO EDIT VIEW OF PRODUCT
function EditProduct(id) {
    debugger;
    window.location.href = "/Test/EditProduct?ProductId=" + id;
}

/////DELETE PRODUCT
function Delete(id) {
    debugger;
    var param = new Object();
    param['ProductId'] = id;


    $.ajax({
        method: "POST",
        url: "/Test/DeleteProduct",
        data: param,
        success: function (data) {
            debugger;
            alert('Product Id ' + data.ProductId + ': Deleted Successfully')
            window.location.href = "/Test/ListOfProduct";
        }
    });
}



$(document).ready(function () {

    debugger;
    $('#tblProduct').DataTable({
        "processing": true,
        "serverSide": true,
        "filter": false,
        "info": false,
        "ordermulti": false,
        "ajax": {
            "url": "/Test/ProductList",
            "type": "POST",
            "datatype": "json"
        },
        "columns": [
            { "data": "ProductId", "name": "ProductId", "autowidth": true },
            { "data": "ProductName", "name": "ProductName", "autowidth": true },
             { "data": "CategoryId", "name": "CategoryId", "autowidth": true },
            { "data": "CategoryName", "name": "CategoryName", "autowidth": true },

            {
                'data': null,
                'render': function (data, type, row) {
                    return '<button id="' + row.ProductId + '" onclick="EditProduct(' + row.ProductId + ')">Edit</button>'
                }
            },
            {
                'data': null,
                'render': function (data, type, row) {
                    return '<button id="' + row.ProductId + '"  onclick="Delete(' + row.ProductId + ')">Delete</button>'
                }
            }

        ]

    })
});
