import React, { useState } from "react";
import ReactSelect from "react-select";

const PlanProcedureItem = ({ procedure, users, assignedUser, onAssignUser }) => {
    const [selectedUsers, setSelectedUsers] = useState([]);

    useEffect(() => {
        if (assignedUsers) {
            const initialSelectedUsers = assignedUsers.map(userId => users.find(user => user.value === userId));
            setSelectedUsers(initialSelectedUsers);
        }
    }, [assignedUsers, users]);

    const handleAssignUserToProcedure = (e) => {
        setSelectedUsers(e);
        // TODO: Remove console.log and add missing logic
        onAssignUser(procedure.procedureId, selectedOption.value);
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
