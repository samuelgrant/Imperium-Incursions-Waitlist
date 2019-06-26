import React, { Component } from 'react';


export default class SelectRoles extends Component {
    updateRoles(e) {
        this.props.selectedRole(e.target.checked, Number(e.target.name));
    }

    render() {
        let roles;
        if (this.props.roles) {
            roles = this.props.roles.map((role) => {
                return (
                    <li className="pb-3">
                        <label className="custom-control custom-checkbox">
                            <input type="checkbox" className="custom-control-input" onChange={this.updateRoles.bind(this)} name={role.id} />
                            <span className="custom-control-indicator"></span>
                            <span className="custom-control-description">{role.name}</span>
                        </label>
                    </li>
                );
            });
        }

        return (
            <div>
                <label>What roles can you help us with?</label>
                <ul className="list-unstyled">
                    {roles}
                </ul>
            </div>
        )
    }
}