import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";
import { addUserToProcedure, deleteUsersFromProcedure } from "../../../api/api";

const PlanProcedureItem = ({plantid procedure, users }) => {
    const [selectedUsers, setSelectedUsers] = useState(null);

    const handleAssignUserToProcedure = (e) => {
        setSelectedUsers(e);
        // TODO: Remove console.log and add missing logic
        console.log(e);
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;