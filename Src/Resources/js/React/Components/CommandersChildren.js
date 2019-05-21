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
        let roles;
        if (!!this.props.user.accountRoles) {
            roles = this.props.user.accountRoles.map((a) => {
                return <p className="m-0">{a.role.name}</p>;
            });
        }

        return (
            <tr>
                <td><img className="img d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" /></td>
                <td><XmppLink AuthName={this.getAccountName()} /></td>
                <td><Corporation corporation={this.getCorporation()} /></td>
                <td><Alliance alliance={this.getAlliance()} /></td>
                <td>{roles}</td>
                <td><button className="btn btn-wl btn-dark btn-sm" onClick={this.props.viewDetails.bind(this, this.props.index)}><i className="fas fa-chevron-double-right"></i></button></td>
            </tr>
        );
    }
}

export class ManageInfo extends Component {
    inputNewFc() {
        return this.props.details == null;
    }

    getPilotUrl() {

        let pilot_id = 0;
        if (this.props.details && this.props.details.pilots[0])
            pilot_id = this.props.details.pilots[0].id;

        return `https://imageserver.eveonline.com/Character/${pilot_id}_128.jpg`;
    }

    getCorporation() {
        if (this.props.details && this.props.details.pilots[0])
            return this.props.details.pilots[0].corporation;

        return null;
    }

    getAlliance() {
        if (this.props.details && this.props.details.pilots[0])
            return this.props.details.pilots[0].corporation.alliance;

        return null;
    }

    render() {

        let fcName;
        if (this.inputNewFc() && this.props.details)
            fcName = this.props.details.name;

        // Text: Account search
        let accountSearch = <Input ref={this.AccountName} id="lookup_account" type="text" classOverride="form-control account-lookup" name="name" required="true" key={null} />
        if (!this.inputNewFc())
            accountSearch = <Input ref={this.AccountName} id="lookup_account" type="text" classOverride="form-control account-lookup" value={this.props.details.name} name="name" disabled="true" required="true" key={this.props.details.id} />

        // String: Date the FC last logged in
        let date;
        if (!this.inputNewFc() && this.props.details.lastLogin) {
            date = DateFormat(this.props.details.lastLogin);
        }

        // Dropdown Item: Roles avaliable to the FC
        let avaliable_roles;        
        if (this.props.roles) {
            avaliable_roles = this.props.roles.map((role) => {
                return <a className="dropdown-item" role="presentation" onClick={this.props.onSubmit.bind(this, role.id)}>{role.name}</a>;
            });
        }

        // Div: List of active roles
        let in_roles;
        if (!this.inputNewFc()) {
            in_roles = this.props.details.accountRoles.map((r) => {
                return <span className="badge role">{r.role.name} <i className="fas fa-times ml-2" onClick={this.props.removeGroup.bind(this, r.role.id)} ></i></span>
            });
        }

        // Button: Reset Button
        let reset_btn = {};
        if (!this.inputNewFc()) {
            reset_btn.large = <button className="btn btn-dark float-left" type="button" onClick={this.props.reset.bind(this, null)}>Back <i className="fas fa-undo-alt"></i></button>;
            reset_btn.small = <i className="fas fa-times-circle float-right mr-3" onClick={this.props.reset.bind(this, null)}></i>
        }

        // Div: Pilot (List of individual pilots, their images and  names)
        let pilotList;
        if (this.props.details && this.props.details.pilots) {
            pilotList = this.props.details.pilots.map((pilot) => {
                return (
                    <span className="pilot">
                        <img src={`https://imageserver.eveonline.com/Character/${pilot.id}_32.jpg`} alt="Pilot Avatar" />
                        <span>{ pilot.name }</span>
                    </span>
                )
            });
        }

        // Div: Pilots (Wrapper around the pilot list, includes heading)
        let pilots;
        if (!this.inputNewFc()) {
            pilots = (
                <div className="ul-pilots">
                    <h4>Pilots</h4>
                    {pilotList}
                </div>
            );
        }

        // Div: Corporation
        let corporation;
        if (!this.inputNewFc()) {
            corporation = (
                <div className="corporation">
                    <h4>Corporation</h4>
                    <img src={`https://imageserver.eveonline.com/Corporation/${this.getCorporation().id}_32.png`} alt="Corporation Logo" />
                    <Corporation corporation={this.getCorporation()} />
                </div>
            )
        }

        // Div: Alliance
        let alliance;
        if (!this.inputNewFc()) {
            alliance = (
                <div className="alliance">
                    <h4>Alliance</h4>
                    <img src={`https://imageserver.eveonline.com/Alliance/${this.getAlliance().id}_32.png`} alt="Alliance Logo" />
                    <Alliance alliance={this.getAlliance()} />
                </div> 
            )
        }

        return (
            <div>
                <div className="panel-body py-4">
                    {reset_btn.small}
                    <img className="rounded-circle d-block mx-auto" src={this.getPilotUrl()} alt="Pilot's Avatar" />

                    <form onSubmit={this.props.onSubmit.bind(this)}>
                        {/* <!-- Account Name --> */}
                        <div className="form-group">
                            <label htmlFor="#lookup_account">GSF Auth Name:</label>
                            {accountSearch}
                            <div className="text-muted mt-1">Last logged in: {date}</div>
                        </div>{/* <!-- End Account Name --> */}

                        {corporation}
                        {alliance}
                        {pilots}

                        {/*<!-- Add to Role -->*/}
                        <hr />
                        <div className="ul-pilots">
                            <h4 className="d-inline-block">Roles</h4>

                            <div className="dropdown ml-3 d-inline-block">
                                <button className="btn btn-dark dropdown-toggle text-left" data-toggle="dropdown" aria-expanded="false" type="button">Assign to Role </button>
                                <div className="dropdown-menu" role="menu">
                                    {avaliable_roles}
                                </div>
                            </div>

                            <div>
                                {in_roles}
                            </div>
                        </div>{/* <!-- End Add to role --> */}
                    </form>
                    {reset_btn.large}
                </div>
            </div>
        )
    }
}