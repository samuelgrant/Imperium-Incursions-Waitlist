import React, { Component } from 'react';
import XmppLink from './XmppLink';
import { DateFormat } from '../Helpers';
import { TextArea, Input } from './FormControls'
import { Corporation, Alliance } from './EsiUi';

export class UserRow extends Component {
    getCorporation() {
        if (this.props.user && this.props.user.pilots[0]) 
            return this.props.user.pilots[0].corporation;
        
        return "";
    }

    getAlliance() {
        if (this.props.user && this.props.user.pilots[0].corporation.alliance)
            return this.props.user.pilots[0].corporation.alliance;

        return "";
    }

    getRoles() {
        return this.props.user.accountRoles.map((a) => {
            return `${a.role.name}\n`;
        });
    }

    // Name of the FCs GICE account
    getAccountName() {       
        return this.props.user.name || "";
    }

    // Pilot image url
    getPilotUrl() {

        let pilot_id = 0;
        if (this.props.user && this.props.user.pilots[0])
            pilot_id = this.props.user.pilots[0].id;

        return `https://imageserver.eveonline.com/Character/${pilot_id}_32.jpg`;
    }

    render() {
        return (
            <tr>
                <td><img className="img d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" /></td>
                <td><XmppLink AuthName={this.getAccountName()} /></td>
                <td><Corporation corporation={this.getCorporation()} /></td>
                <td><Alliance alliance={this.getAlliance()} /></td>
                <td>{this.getRoles()}</td>
                <td><button className="btn btn-wl btn-dark btn-sm" onClick={this.props.viewDetails.bind(this, this.props.index)}><i className="fas fa-chevron-double-right"></i></button></td>
            </tr>
        );
    }
}
