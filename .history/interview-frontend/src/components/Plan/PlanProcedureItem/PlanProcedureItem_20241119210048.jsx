import React, { useState, useEffect } from "react";
import ReactSelect from "react-select";
import { addUserToProcedure, deleteUsersFromProcedure } from "../../../api/api";

const PlanProcedureItem = ({ planId, procedure, users }) => {
    const [selectedUsers, setSelectedUsers] = useState([]);

    useEffect(() => {
        if (procedure && procedure.planProcedureUsers && procedure.planProcedureUsers.length > 0) {
            const ppUsers = procedure.planProcedureUsers.filter(ppu => ppu.procedureId === procedure.procedureId);
            if (ppUsers && ppUsers.length > 0) {
                const pUsers = ppUsers.map(ppu => users.find(u => u.value === ppu.userId)).filter(Boolean);
                setSelectedUsers(pUsers);
            }
        }
    }, [planId, procedure, users]);

    const handleAssignUserToProcedure = async (e) => {
        const newSelectedUsers = e || [];
        const newUserIds = newSelectedUsers.map(user => user.value);
        const previousUserIds = selectedUsers.map(user => user.value);

        const addedUsers = newUserIds.filter(userId => !previousUserIds.includes(userId));
        const removedUsers = previousUserIds.filter(userId => !newUserIds.includes(userId));

        setSelectedUsers(newSelectedUsers);

        try {
            if (addedUsers.length > 0) {
                await addUserToProcedure(planId, procedure.procedureId, addedUsers);
            }

            if (removedUsers.length > 0) {
                await deleteUsersFromProcedure(planId, procedure.procedureId, removedUsers);
            }
        } catch (error) {
            console.error("Error updating users:", error);
        }
    };

    return (
        <div className="py-2">
            <div>{procedure.procedureTitle}</div>
            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={handleAssignUserToProcedure}
            />
        </div>
    );
};

export default PlanProcedureItem;
