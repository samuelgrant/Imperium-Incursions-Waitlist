import React, { Component } from 'react';
import XmppLink from './XmppLink';
import { DateFormat } from '../Helpers';
import { TextArea, Input } from './FormControls'

export class BanRow extends Component {
    //Permanant or temporary
    getBanType() {
        if(!this.props.ban.expiresAt)
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

    getPilotUrl() {

        let pilot_id = 0;
        if (this.props.ban && this.props.ban.bannedAccount.pilots[0])
            pilot_id = this.props.ban.bannedAccount.pilots[0].id;

        return `https://imageserver.eveonline.com/Character/${pilot_id}_32.jpg`;
    }

    render() {
        return (
            <tr>
                <td><img className="img d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" /></td>
                <td><XmppLink AuthName={this.getBaneeName()} /></td>
                <td><XmppLink AuthName={this.getAdminName()} /></td>
                <td>{this.getBanType()}</td>
                <td><button className="btn btn-wl btn-success btn-sm" onClick={this.props.revokeBan.bind(this, this.getBanId())}>Revoke Ban <i className="fas fa-gavel"></i></button></td>
                <td><button className="btn btn-wl btn-dark btn-sm" onClick={this.props.viewDetails.bind(this, this.props.index)}><i className="fas fa-chevron-double-right"></i></button></td>
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
        if (this.props.details && this.props.details.bannedAccount.pilots[0]) 
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
                    <Input id="createdAt" type="text" value={DateFormat(this.props.details.createdAt)} disabled="true" key={this.props.details.id}/>
                </div>
            )
        }

        // Text: Account search
        let accountSearch = <Input id="lookup_account" type="text" classOverride="form-control account-lookup" name="name" required="true" key={null}/>
        if (!this.inputNewBan()) {
            accountSearch = <Input id="lookup_account" type="text" classOverride="form-control account-lookup" value={this.props.details.bannedAccount.name} name="name" disabled="true" required="true" key={this.props.details.id}/>
        }

        // Textarea: Ban Reason
        let reason = <TextArea id="banReason" name="reason" required="true" key={null}/>
        if (!this.inputNewBan()) {
            reason = <TextArea id="banReason" name="reason" value={this.props.details.reason} required="true" key={this.props.details.id}/>;
        }

        // Button: Reset Button
        let reset_btn;
        if (!this.inputNewBan()) {
            reset_btn = <button className="btn btn-dark float-left" type="button" onClick={this.props.reset.bind(this, null)}>Reset <i className="fas fa-undo-alt"></i></button>;
        }

        return (
            <div>
                <div className="panel-body py-4">
                    <img className="rounded-circle d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" />

                    <form onSubmit={this.props.onSubmit.bind(this)}>
                        <div className="form-group">
                            <label htmlFor="#lookup_account">GSF Auth Name:</label>
                            {accountSearch}
                        </div>
                        
                        {banIssuedAt}

                        <div className="form-group">
                            <label htmlFor="banExpires">Ban Expires:</label>
                            <Input type="text" id="banExpires" disabled="true"/>
                            <small className="text-muted">Bans expire at downtime, leave blank for permanant</small>
                        </div>

                        <div className="form-group">
                            <label htmlFor="banReason">Reason:</label>
                            {reason}
                            <small className="text-muted">Only visible to the FC team</small>
                        </div>

                        {reset_btn}
                        <button className="btn btn-danger float-right" type="submit">Ban User <i className="fas fa-gavel"></i></button>
                    </form>
                </div>
            </div>
        )
    }
}