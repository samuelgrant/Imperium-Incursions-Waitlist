import React, { Component } from 'react';
import { Input } from '../FormControls';
import AccountPilots from './AccountPilots';
import Roles from './Roles';
import { DateFormat, AccountPilot } from '../../Helpers';
import { Corporation, Alliance } from '../EsiUi';


export default class UserInfo extends Component {
    // The next three methods are needed to make autocomplete work when the component is controlled by a key
    componentDidUpdate() { this.configureAutocomplete() }
    componentDidMount() { this.configureAutocomplete() }

    configureAutocomplete() {
        $("#lookup_account").autocomplete({
            source: (request, response) => {
                $.ajax({
                    url: `/search?q=${request.term}&filter=account`,
                    dataType: "json",
                    success: function (data) {
                        response(data);
                    }
                });
            },
            minLength: 3,
            delay: 500
        })
    }

    inputNewFc() {
        return this.props.selectedUser == null;
    }

    getPilotUrl() {
        let pilot_id = 0;
        if (this.props.selectedUser)
            pilot_id = AccountPilot(this.props.selectedUser.name, this.props.selectedUser.pilots).id;

        return `https://imageserver.eveonline.com/Character/${pilot_id}_128.jpg`;
    }

    getCorporation() {
        if (this.props.selectedUser && this.props.selectedUser.pilots)
            return AccountPilot(this.props.selectedUser.name, this.props.selectedUser.pilots).corporation;

        return null;
    }

    getAlliance() {
        if (this.props.selectedUser && this.props.selectedUser.pilots)
            return AccountPilot(this.props.selectedUser.name, this.props.selectedUser.pilots).alliance;

        return null;
    }

    addRole(roleId) {
        if (roleId == null) {
            console.error(`[React/UserInfo] @addRole - You must supply a role ID`);
            return;
        }

        $.ajax({
            type: 'post',
            url: `${this.props.baseUri}`,
            data: {
                account_id: this.props.selectedUser ? this.props.selectedUser.id : null,
                role_id: roleId,
                account_name: $("#lookup_account").val()
            }
        }).done((data) => {
            let index = this.props.userIndex;
            this.props.u();
            this.props.setIndex(index);
        }).fail((err) => {
            console.error(`[React/UserInfo] @addRole - Error adding role`, err.responseText);
        })
    }

    render() {
        // Button: Reset Button
        let reset_btn = {};
        if (!this.inputNewFc()) {
            reset_btn.large = <button className="btn btn-dark float-left mt-3" type="button" onClick={this.props.setIndex.bind(this, null)}>Back <i className="fas fa-undo-alt"></i></button>;
            reset_btn.small = <i className="fas fa-times-circle float-right mr-3 i-btn" onClick={this.props.setIndex.bind(this, null)}></i>
        }

        // Only shown when viewing/editing an FC
        let corporation;
        if (!this.inputNewFc()) {
            corporation = (
                <div className="corporation">
                    <h4>Corporation</h4>
                    <img src={`https://imageserver.eveonline.com/Corporation/${(this.getCorporation()) ? this.getCorporation().id : 0}_32.png`} alt="Corporation Logo" />
                    <Corporation corporation={this.getCorporation()} />
                </div>
            )
        }

        let alliance;
        if (!this.inputNewFc()) {
            alliance = (
                <div className="alliance">
                    <h4>Alliance</h4>
                    <img src={`https://imageserver.eveonline.com/Alliance/${(this.getAlliance()) ? this.getAlliance().id : 0}_32.png`} alt="Alliance Logo" />
                    <Alliance alliance={this.getAlliance()} />
                </div>
            )
        }

        let avaliable_roles;
        if (this.props.availableRoles) {
            avaliable_roles = this.props.availableRoles.map((role) => {
                return <a className="dropdown-item" role="presentation" onClick={this.addRole.bind(this, role.id)}>{role.name}</a>;
            });
        }

        let addRoles;
        if (this.props.availableRoles) {
            addRoles = (
                <div className="input-group-append">
                    <div className="dropdown">
                        <button className="btn btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Add to Role... </button>
                        <div className="dropdown-menu" role="menu">
                            {avaliable_roles}
                        </div>
                    </div>
                </div>
            )
        }

        let roles;
        let pilots;
        if (!this.inputNewFc()) {
            roles = (
                <Roles user={this.props.selectedUser ? this.props.selectedUser : null}
                    baseUri={this.props.baseUri}
                    u={this.props.u.bind(this)}
                    userIndex={this.props.userIndex}
                    setIndex={this.props.setIndex.bind(this)} />
            )
            pilots = <AccountPilots pilots={this.props.selectedUser ? this.props.selectedUser.pilots : null} />
        }

        return (
            <div className="panel-body py-4">
                {reset_btn.small}
                <img className="rounded-circle d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" />

                <form>
                    <div className="form-group">
                        <label htmlFor="#lookup_account">GSF Auth Name:</label>
                        <div className="input-group">
                            <Input id="lookup_account"
                                type="text"
                                name="name"
                                required="true"
                                value={this.props.selectedUser ? this.props.selectedUser.name : ""}
                                disabled={this.props.selectedUser ? "true" : "false"}
                                key={this.props.selectedUser ? this.props.selectedUser.id : null} />

                            {addRoles}
                        </div>

                        <div className="text-muted mt-1">Last logged in: {this.props.selectedUser ? DateFormat(this.props.selectedUser.lastLogin) : ""}</div>
                    </div>

                    <div className="row">
                        <div className="col-lg-6 col-md-12">
                            {corporation}
                        </div>
                        <div className="col-lg-6 col-md-12">
                            {alliance}
                        </div>
                    </div>

                    {pilots}
                    {roles}

                    {reset_btn.large}
                </form>
            </div>
        )
    }
}