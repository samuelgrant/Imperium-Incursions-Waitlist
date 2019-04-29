import React, { Component } from 'react';
import XmppLink from './XmppLink';
import { Account } from './AutocompleteInputs';
import { DateFormat } from '../Helpers';

import { TextArea, Input } from './FormControls'

export class BanRow extends Component {
    //Permanant or temporary
    getBanType() {
        if(this.props.ban.expiresAt)
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
        return this.props.ban.creatorAdmin.name || "";
    }

    render() {
        return (
            <tr>
                <td>img</td>
                <td><XmppLink AuthName={this.getBaneeName()} /></td>
                <td><XmppLink AuthName={this.getAdminName()} /></td>
                <td>{this.getBanType()}</td>
                <td><button className="btn btn-wl btn-info" onClick={this.props.viewDetails.bind(this, this.props.index)}><i className="fas fa-info-circle"></i></button></td>
                <td><button className="btn btn-wl btn-success" onClick={this.props.revokeBan.bind(this, this.getBanId())}>Revoke Ban <i className="fas fa-gavel"></i></button></td>
            </tr>
        );
    }
}

export class ManageInfo extends Component {

    inputNewBan() {
        return this.props.details == null;
    }

    getPilotUrl() {
        
        let pilot_id = 0;
        if (this.props.details && this.props.details.bannedAccount) 
            pilot_id = this.props.details.bannedAccount.pilots[0].id;

        return `https://imageserver.eveonline.com/Character/${pilot_id}_128.jpg`;
    }

    getReason() {
        if(this.props.details && this.props.details.reason)
         console.log(this.props.details.reason);
        return (this.props.details && this.props.details.reason) ? this.props.details.reason : "";
    }

    getBaneeName() {
        return (!this.inputNewBan()) ? this.props.details.bannedAccount.name : "";
    }

    render() {
        let banIssuedAt;
        if (!this.inputNewBan()) {
            banIssuedAt = (
                <div className="form-group">
                    <label htmlFor="createdAt">Ban issued:</label>
                    <Input id="createdAt" type="text" value={DateFormat(this.props.details.createdAt)} disabled="true" />
                </div>
            )
        }

        let accountSearch = <Input id="lookup_account" type="text" classOverride="form-control account-lookup" name="name" required="true" />
        if (!this.inputNewBan()) {
            accountSearch = <Input id="lookup_account" type="text" classOverride="form-control account-lookup" value={this.props.details.bannedAccount.name} name="name" required="true" />
        }

        // Textarea: Ban Reason
        let reason = <TextArea id="banReason" name="reason"/>
        if (!this.inputNewBan()) {
            reason = <TextArea id="banReason" name="reason" value={this.props.details.reason}/>;
        }

        // Panel Heading
        let headingText = "New Ban";
        if (!this.inputNewBan()) {
            headingText = "Viewing Ban";
        }

        return (
            <div className="side-panel">
                <div className="panel-heading">
                    {headingText}
                </div>

                <div className="panel-body">
                    <img className="rounded-circle d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" />

                    <form onSubmit={this.props.onSubmit.bind(this)}>
                        <div className="form-group">
                            <label htmlFor="#lookup_account">GSF Auth Name:</label>
                            {accountSearch}
                        </div>
                        
                        {banIssuedAt}

                        <div className="form-group">
                            <label htmlFor="banDuration">Ban duration (in days):</label>
                            <Input type="number" id="banDuration"/>
                            <small className="text-muted">Leave blank for permanant</small>
                        </div>

                        <div className="form-group">
                            <label htmlFor="banReason">Reason:</label>
                            {reason}
                            <small className="text-muted">Only visible to the FC team</small>
                        </div>
                        <button className="btn btn-primary" type="button" onClick={this.props.reset.bind(this, null)}>Reset</button>
                        <button className="btn btn-danger">Ban User <i className="fas fa-gavel"></i></button>
                    </form>
                </div>
            </div>
        )
    }
}