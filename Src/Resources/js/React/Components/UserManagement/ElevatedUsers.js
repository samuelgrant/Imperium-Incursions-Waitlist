import React, { Component } from 'react';
import { AccountPilot } from '../../Helpers';
import { XmppLink } from '../CommLinks';
import { Corporation, Alliance } from '../EsiUi';


export default class SpecialUsers extends Component {
    render() {
        let users;
        if (!!this.props.users) {
            users = this.props.users.map((user, index) => {
                return <UserRow user={user} setIndex={this.props.setIndex.bind(this, index)} />
            });
        }

        return (
            <table className="table table-responsive">
                <thead>
                    <tr className="font-alpha">
                        <th></th>
                        <th>Name</th>
                        <th>Corporation</th>
                        <th>Alliance</th>
                        <th>Roles</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {users}
                </tbody>
            </table>
        )
    }
}

export class UserRow extends Component {
    getCorporation() {
        if (this.props.user && this.props.user.pilots)
            return AccountPilot(this.props.user.name, this.props.user.pilots).corporation;

        return;
    }

    getAlliance() {
        if (this.props.user && this.props.user.pilots)
            return AccountPilot(this.props.user.name, this.props.user.pilots).alliance;

        return;
    }

    // Name of the FCs GICE account
    getAccountName() {
        return this.props.user.name || "";
    }

    // Pilot image url
    getPilotUrl() {
        let pilot_id = 0;
        if (this.props.user)
            pilot_id = AccountPilot(this.props.user.name, this.props.user.pilots).id;

        return `https://imageserver.eveonline.com/Character/${pilot_id}_32.jpg`;
    }

    render() {
        let roles;
        if (!!this.props.user.roles) {
            roles = this.props.user.roles.map((role) => {
                return <p className="m-0">{role.name}</p>;
            });
        }

        return (
            <tr>
                <td><img className="img d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" /></td>
                <td><XmppLink AuthName={this.getAccountName()} /></td>
                <td><Corporation corporation={this.getCorporation()} /></td>
                <td><Alliance alliance={this.getAlliance()} /></td>
                <td>{roles}</td>
                <td><button className="btn btn-wl btn-dark btn-sm" onClick={this.props.setIndex.bind(this, this.props.index)}><i className="fas fa-chevron-double-right"></i></button></td>
            </tr>
        );
    }
}