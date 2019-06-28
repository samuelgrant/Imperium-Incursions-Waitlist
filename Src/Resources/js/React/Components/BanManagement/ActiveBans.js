import React, { Component } from 'react';
import { AccountPilot } from '../../Helpers';
import { XmppLink } from '../CommLinks';

export default class ActiveBans extends Component {
    revoke(banId) {
        $.ajax({
            type: 'delete',
            url: `${this.props.baseUri}/${banId}`
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`[React/ActiveBans] @revoke - Error revoking ban (ban Id: ${banId})`, err.responseText);
        });
    }

    render() {
        let bans;
        if (!!this.props.bans) {
            bans = this.props.bans.map((ban, index) => {
                return <BanRow ban={ban} revokeBan={this.revoke.bind(this, ban.id)} setIndex={this.props.setIndex.bind(this, index)} admin={this.props.admin} />
            });
        }

        return (
            <table className="table table-responsive">
                <thead>
                    <tr className="font-alpha">
                        <th></th>
                        <th>Name</th>
                        <th>Admin</th>
                        <th></th>
                        <th></th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {bans}
                </tbody>
            </table>
        )
    }
}

export class BanRow extends Component {
    //Permanant or temporary
    getBanType() {
        if (!this.props.ban.expiresAt)
            return <span className="text-danger">Permanant Ban</span>;

        return <span className="text-white">Temporary Ban</span>
    }

    getBanId() {
        return this.props.ban.id || null;
    }

    //Name of the banned account
    getBaneeName() {
        return this.props.ban.bannedAccount.name || "";
    }

    //Name of the last admin who issued a ban
    getAdminName() {
        return this.props.ban.banAdmin.name || "";
    }

    getPilotUrl() {
        let pilot_id = 0;
        if (this.props.ban && this.props.ban.bannedAccount.pilots)
            pilot_id = AccountPilot(this.props.ban.bannedAccount.name, this.props.ban.bannedAccount.pilots).id;

        return `https://imageserver.eveonline.com/Character/${pilot_id}_32.jpg`;
    }

    render() {
        let btn_revoke;
        if (this.props.admin) {
            btn_revoke = <button className="btn btn-wl btn-success btn-sm" onClick={this.props.revokeBan}>Revoke Ban <i className="fas fa-gavel"></i></button>
        }

        return (
            <tr>
                <td><img className="img d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" /></td>
                <td><XmppLink AuthName={this.getBaneeName()} /></td>
                <td><XmppLink AuthName={this.getAdminName()} /></td>
                <td>{this.getBanType()}</td>
                <td>{btn_revoke}</td>
                <td><button className="btn btn-wl btn-dark btn-sm" onClick={this.props.setIndex}><i className="fas fa-chevron-double-right"></i></button></td>
            </tr>
        );
    }
}