import React, { Component } from 'react';

export default class Roles extends Component {
    removeRole(i) {
        $.ajax({
            type: 'delete',
            url: `${this.props.baseUri}/revoke`,
            data: {
                roleId: i,
                accountId: this.props.user.id || null
            }
        }).done((result) => {
            let index = this.props.userIndex;
            this.props.u();
            this.props.setIndex(index);
        }).fail((err) => {
            console.error(`[React/Roles] @removeRole - Error revoking role:`, err.responseText);
        });
    }

    render() {
        let in_roles;
        if (this.props.user) {
            in_roles = this.props.user.roles.map((role) => {
                return <span className="badge role">{role.name} <i className="fas fa-times ml-2" onClick={this.removeRole.bind(this, role.id)}></i></span>
            });
        }

        return (
            <div>
                <h4 className="d-inline-block pb-2">Roles</h4>
                <div className="d-block">
                    {in_roles}
                </div>
            </div>
        )
    }
}