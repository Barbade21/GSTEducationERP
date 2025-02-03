//this is the script to get the applied tax from the from the database
function getappliectax(hsncode, row, vendorcode) {
    $.get('/Accountant/GetAppliedtaxAsyncVP', { hsncode: hsncode, VendorCode: vendorcode }, function (response) {
        if (response.success) {
            row.find('.AppliedTax select').empty();

            //row.find('.AppliedTax select').append('<option value="">Select Tax</option>');
            $.each(response.taxOptions, function (index, tax) {
                row.find('.AppliedTax select').append('<option value="' + tax.value + '">' + tax.text + '</option>');
            });
        } else {
            row.find('.AppliedTax select').empty().append('<option value="">No Tax Found</option>');
        }
    }).fail(function (error) {
        console.error("Error fetching tax options:", error);
    });

}
//this fuction is for calculating the balance
function calculateBalance() {
    let balamount = 0;
    const Totalamount = parseFloat(document.getElementById("pymtotalamt").innerText);
    const Paidamount = $("#TransactionAmount").val();
    if (Paidamount == Totalamount) {
        //document.getElementById("BalanceAmount").innerText = 0;
        document.getElementsByName('BalanceAmount')[0].value = 0;
        document.getElementsByName('StatusId')[0].value = 6;
        // document.getElementById("StatusId").innerText = 6;
    }
    else {

        balamount = Totalamount - Paidamount;
        if (balamount < -1) { balamount = 0; }
        //document.getElementById("BalanceAmount").innerText = balamount.toFixed(2);
        document.getElementsByName('BalanceAmount')[0].value = balamount.toFixed(2);
        document.getElementsByName('StatusId')[0].value = 6;
        //document.getElementById("StatusId").innerText = 6;
    }
}
//function for adding the row to table
//function addrow() {
//    var $clone = $("#tblpurchaseitem tbody tr:first").clone();

//    $clone.find('input').val('');
//    $clone.find('select').prop('selectedIndex', 0);

//    $clone.find('.remove').html(`
//                    <a href="javascript:void(0);" class="icon" onclick="removeRow(this, $(this).closest('tr').find('.ItemId input').val())">
//                        <video width="30" playsinline loop>
//                            <source src="@Url.Content("~/Content/Admin/icon/Delete%202.mp4")" type="video/mp4" />
//                            Your browser does not support the video tag.
//                        </video>
//                    </a>
//                `);

//    $("#tblpurchaseitem tbody").append($clone);

//    $("#tblpurchaseitem tbody tr").each(function (index) {
//        $(this).find('td:first').text(String(index + 1).padStart(2, '0'));
//    });
//}
//validatin the row
function validateRow() {
    var isValid = true;
    $('#tblpurchaseitem tbody tr:last').find('input, select').each(function () {
        if ($(this).is(':visible') && $(this).val() === '') {
            $(this).css('border', '1px solid red');
            isValid = false;
        } else {
            $(this).css('border', '');
        }
    });
    if (!isValid) {
        Swal.fire({
            title: 'Invalid Input',
            text: 'Please fill the all the item details..!',
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }
    return isValid;
}
///function for removing the row
function removeRow(element, itemId) {
    if (itemId === "") {
        $(element).closest('tr').remove();
    } else {
        // Show SweetAlert confirmation dialog
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Accountant/DeletePurchaseItemAsyncVP',
                    type: 'POST',
                    data: { itemId: itemId },
                    success: function (response) {
                        if (response.success) {
                            $(element).closest('tr').remove();
                            displayToast('Item successfully deleted.', "success");
                        } else {
                            displayToast('Failed to delete the item.', "error");
                        }
                    },
                    error: function () {
                        displayToast('An error occurred while deleting the item.', "error");
                    }
                });
            }
        });
    }
}

//function for validating the submit criteria 

//save purchase here
async function savePurchase() {
    try {
        // Update the BalanceAmount before creating the FormData object
        updateTotals();
        // Create FormData object
        var formData = new FormData(document.getElementById('addpurchaseAsyncVP'));
        formData.append('BalanceAmount', $('#BalanceAmount').val());
        formData.append('VendorCode', $('#VendorCode option:selected').val());
        let errormsg = "";

        const addPurchaseResponse = await $.ajax({
            type: 'POST',
            url: '/Accountant/AddPurchaseAsyncVP',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                } else {
                    errormsg = "Error in saving purchase details";
                    throw new Error(errormsg);
                }
            },
            error: function (xhr, status, error) {
                errormsg = "AJAX error: " + error;
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: errormsg,
                    showConfirmButton: true
                });
                throw new Error(errormsg);
            }
        });

        if (addPurchaseResponse.success) {
            const incrementcode = addPurchaseResponse.PurchaseCode;
            var PurchaseItemsVPModels = ListPurchaseItemForSave(incrementcode);
            const addPurchaseItemResponse = await $.ajax({
                url: '/Accountant/AddPurchaseItemAsyncVP',
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json',
                data: JSON.stringify({ PurchaseItemsAsyncVP: PurchaseItemsVPModels }),
                success: function (response) {
                    if (response.success) {
                    } else {
                        errormsg = "Error in saving purchase items";
                        throw new Error(errormsg);
                    }
                },
                error: function (xhr, status, error) {
                    errormsg = "AJAX error: " + error;
                    Swal.fire({
                        position: "center",
                        icon: "error",
                        title: errormsg,
                        showConfirmButton: true
                    });
                    throw new Error(errormsg);
                }
            });

            if (addPurchaseItemResponse.success) {
                return true;
            } else {
                errormsg = "Error in saving purchase items";
                Swal.fire({
                    position: "center",
                    icon: "error",
                    title: errormsg,
                    showConfirmButton: true
                });
                return false;
            }
        } else {
            Swal.fire({
                position: "center",
                icon: "error",
                title: errormsg,
                showConfirmButton: true
            });
            return false;
        }

    } catch (error) {
        console.error("Error occurred: ", error);
        Swal.fire({
            position: "center",
            icon: "error",
            title: "Error: " + error.message,
            showConfirmButton: true
        });
        return false;
    }
}

///getting thepurchased itme list in the one object for update
function ListPurchaseItem() {
    var PurchaseItemsVPModels = [];
    $("#tblpurchaseitem tbody tr").each(function () {
        var ItemId = $(this).find(".ItemId input").val();
        var TransactionCode = $('#TransactionCode').val();;
        var ItemName = $(this).find(".ItemName input").val();
        var HSNCode = $(this).find(".HSNCode select").val();
        var unit = $(this).find(".Quantity select").val();
        var Quantity = $(this).find(".Quantity input").val();
        var UnitPrice = $(this).find(".UnitPrice input").val();
        var Discount = $(this).find(".Discount input").val();
        var AppliedTax = $(this).find(".AppliedTax select option:selected").val();

        var PurchaseItemsVP = {
            ItemId: ItemId,
            TransactionCode: TransactionCode,
            ItemName: ItemName,
            HSNCode: HSNCode,
            Unit: unit,
            Quantity: Quantity,
            UnitPrice: UnitPrice,
            Discount: Discount,
            AppliedTax: AppliedTax
        };

        PurchaseItemsVPModels.push(PurchaseItemsVP);
    });
    return PurchaseItemsVPModels;
}
//function for getting the purchased itmes list in one object for save
function ListPurchaseItemForSave(code) {
    var PurchaseItemsVPModels = [];
    $("#tblpurchaseitem tbody tr").each(function () {
        var ItemId = $(this).find(".ItemId input").val();
        var TransactionCode = code;
        var ItemName = $(this).find(".ItemName input").val();
        var HSNCode = $(this).find(".HSNCode select").val();
        var unit = $(this).find(".Quantity select").val();
        var Quantity = $(this).find(".Quantity input").val();
        var UnitPrice = $(this).find(".UnitPrice input").val();
        var Discount = $(this).find(".Discount input").val();
        var AppliedTax = $(this).find(".AppliedTax select option:selected").val();

        var PurchaseItemsVP = {
            ItemId: ItemId,
            TransactionCode: TransactionCode,
            ItemName: ItemName,
            HSNCode: HSNCode,
            Unit: unit,
            Quantity: Quantity,
            UnitPrice: UnitPrice,
            Discount: Discount,
            AppliedTax: AppliedTax
        };

        PurchaseItemsVPModels.push(PurchaseItemsVP);
    });
    return PurchaseItemsVPModels;
}
//function for fetching the voucher here

/* < !--fuction for toastr-- >*/



//function for validating the payment
function validatePayment() {
    var TransactionAmount = $("#TransactionAmount").val();
    var amounttopay = $("#pymtotalamt").text();
    var voucheramount = $("#totalAmountLabel").text();
    var VoucherCode = $('#VoucherCode option:selected').val();
    const desceription = $("#txtDescription").val();
    //Validation for required fields
    if (!TransactionAmount || amounttopay === "0" || !voucheramount === "0" || !VoucherCode || !desceription) {
        let errormsg = "";
        (TransactionAmount == 0) || (TransactionAmount > voucheramount) ? message = "Please provid the appropriate amount.!" : null;
        !desceription ? message = "Please provide the description about the payment description.!" : null;
        !voucheramount ? message = "Please select the voucher for payment !" : null;

        Swal.fire({
            title: 'please fill the appropriate date',
            text: message,
            icon: 'warning',
            confirmButtonText: 'OK'
        });
        return false;
    }
    return true;
}
function addrowupdate() {
    var $clone = $("#tblpurchaseitem tbody tr:first").clone();

    $clone.find('input').val('');
    $clone.find('select').prop('selectedIndex', 0);
    $clone.find('.remove').html(`
                   <a href="javascript:void(0);" class="icon" onclick="removeRow(this, $(this).closest('tr').find('.ItemId input').val())">
                        <video width="25" height="25" playsinline loop>
                            <source src="@Url.Content("~/Content/Admin/icon/Delete%202.mp4")" type="video/mp4" />
                            Your browser does not support the video tag.
                        </video>
                    </a>
                `);
    $("#tblpurchaseitem tbody").append($clone);
    $("#tblpurchaseitem tbody tr").each(function (index) {
        $(this).find('td:first').text(String(index + 1).padStart(2, '0'));
    });
   
}


//function for updating the total
async function updatePurchase() {
    try {
        updateTotals();
        var formData = getFormData();
        formData.append('VendorName', $('#VendorName option:selected').val());
        const updatePurchaseResponse = await $.ajax({
            type: 'POST',
            url: '/Accountant/UpdatePurchaseAsyncVP',
            data: formData,
            contentType: false,
            processData: false
        });

        if (updatePurchaseResponse.success) {
            displayToast("Update purchase success", "success");

        } else {
            displayToast("Error in saving purchase details", "error");
            return false;
        }

        var PurchaseItemsVPModels = ListPurchaseItem();

        const updatePurchaseItemResponse = await $.ajax({
            url: '/Accountant/UpdatePurchaseItemAsyncVP',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify({ PurchaseItemsAsyncVP: PurchaseItemsVPModels })
        });

        if (updatePurchaseItemResponse.success) {
            displayToast("Update items successfully!", "success");
            return true;
        } else {
            displayToast("Error in saving purchase item", "error");
            return false;
        }
    } catch (error) {
        //console.error('Error in updatePurchase:', error);
        displayToast("Unexpected error occurred", "error");
        return false;
    }
}
//getting the form data
function getFormData() {
    var form = document.getElementById("addpurchaseAsyncVP");
    var formData = new FormData(form);
    return formData;
}
//----------------------------making the table dynamic---------------------------------------//
// Function to hide the HSN Code, Discount, and Applied Tax columns
function hideGSTColumns() {
    // Hide header columns
    $("th:contains('HSN Code'), th:contains('Discount'), th:contains('Applied Tax')").hide();
    // Hide body columns
    $(".HSNCode, .Discount, .AppliedTax").hide();
}

// Function to show the HSN Code, Discount, and Applied Tax columns
function showGSTColumns() {
    // Show header columns
    $("th:contains('HSN Code'), th:contains('Discount'), th:contains('Applied Tax')").show();
    // Show body columns
    $(".HSNCode, .Discount, .AppliedTax").show();
}
function forRadioButton() {
    if ($('#ishsnapplied').is(":checked")) {
        showGSTColumns();
    } else {
        hideGSTColumns();
    }
    updateTotals();
}