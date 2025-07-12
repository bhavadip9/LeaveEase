
$(document).ready(function () {


    function updatePermissionColumn(permissionDetails, colIndex) {
        let canViewCheckbox = permissionDetails.find("tr").eq(0).find("td").eq(colIndex + 1).find("input.toggle-switch");
        let canAddEditCheckbox = permissionDetails.find("tr").eq(1).find("td").eq(colIndex + 1).find("input.toggle-switch");
        let canDeleteCheckbox = permissionDetails.find("tr").eq(2).find("td").eq(colIndex + 1).find("input.toggle-switch");

        if (canViewCheckbox.prop("disabled")) {   
            return;
        }

        if (!canViewCheckbox.is(":checked")) {
            canAddEditCheckbox.prop("checked", false).prop("disabled", true);
            canDeleteCheckbox.prop("checked", false).prop("disabled", true);
        } else {
            canAddEditCheckbox.prop("disabled", false);
            canDeleteCheckbox.prop("disabled", false);
        }
    }

    // When CanView changes, update the same column
    $(".toggle-switch[data-action='CanView']").change(function () {
        let checkbox = $(this);
        if (checkbox.prop("disabled")) return;

        let permissionDetails = checkbox.closest(".permission-details").find("table tbody");
        let td = checkbox.closest("td");
        let colIndex = td.index() - 1;
        updatePermissionColumn(permissionDetails, colIndex);
    });

    // When CanAddEdit or CanDelete changes, make sure CanView is checked
    $(".toggle-switch[data-action='CanAddEdit'], .toggle-switch[data-action='CanDelete']").change(function () {
        let checkbox = $(this);
        if (checkbox.prop("disabled")) return; // skip disabled fields

        if (checkbox.is(":checked")) {
            let permissionDetails = checkbox.closest(".permission-details").find("table tbody");
            let td = checkbox.closest("td");
            let colIndex = td.index() - 1;

            let canViewCheckbox = permissionDetails.find("tr").eq(0).find("td").eq(colIndex + 1).find("input.toggle-switch");
            if (!canViewCheckbox.is(":checked")) {
                canViewCheckbox.prop("checked", true).trigger("change"); // also trigger change so it updates rest
            }
        }
    });

    // On page load
    $(".permission-details").each(function () {
        let permissionDetails = $(this).find("table tbody");
        let colCount = permissionDetails.find("tr").first().find("td").length;

        for (let colIndex = 0; colIndex < colCount - 1; colIndex++) {
            updatePermissionColumn(permissionDetails, colIndex);
        }
    });

    $(".permission-toggle").on("click", function () {
        const $icon = $(this).find(".expand-icon");
        $(this).toggleClass("active");
        const $details = $(this).next(".permission-details");

        $details.toggleClass("d-none");
        $icon.toggleClass("rotate");
    });
    $('#savePermissions').click(function () {
        let data = [];

        $('.toggle-switch').each(function () {
            let permissionId = $(this).data('permission-id');
            let roleId = $(this).data('role-id');
            let action = $(this).data('action');
            let isChecked = $(this).is(':checked');

            let existing = data.find(d => d.permissionId === permissionId && d.roleId === roleId);
            if (!existing) {
                existing = {
                    permissionId: permissionId,
                    roleId: roleId,
                    canView: false,
                    canAddEdit: false,
                    canDelete: false
                };
                data.push(existing);
            }

            if (action === 'CanView') existing.canView = isChecked;
            if (action === 'CanAddEdit') existing.canAddEdit = isChecked;
            if (action === 'CanDelete') existing.canDelete = isChecked;
 
        });

        $.ajax({
            url: '/Permission/SavePermissions',
            type: 'POST',
            data: JSON.stringify(data),
            contentType: 'application/json',
            success: function (response) {
               
                setTimeout(function () {             
                    location.reload(true);
                }, 1000);
               
                toastr.success("Permissions Update successfully!")

            },
            error: function (xhr) {
                toastr.success("Error saving permissions.")

            }
        });
    });
});