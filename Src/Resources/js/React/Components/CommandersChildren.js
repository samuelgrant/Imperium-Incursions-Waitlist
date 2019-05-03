import React, { Component } from 'react';
import XmppLink from './XmppLink';
import { DateFormat } from '../Helpers';
import { TextArea, Input } from './FormControls'

export class UserRow extends Component {
    getCorporation() {
        if (this.props.user && this.props.user.pilots[0]) 
            return this.props.user.pilots[0].corporationId;
        
        return "";
    }

    getAlliance() {
        if (this.props.user && this.props.user.pilots[0])
            return this.props.user.pilots[0].corporationId;

        return "";
    }

    getRoles() {
        return this.props.user.accountRoles.map((a, index) => {
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
                <td>{this.getCorporation()}</td>
                <td>{this.getAlliance()}</td>
                <td>{this.getRoles()}</td>
                <td><button className="btn btn-wl btn-dark btn-sm" onClick={this.props.viewDetails.bind(this, this.props.index)}><i className="fas fa-chevron-double-right"></i></button></td>
            </tr>
        );
    }
}
