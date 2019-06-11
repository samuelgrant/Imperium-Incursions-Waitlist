import React, { Component } from 'react';
import { SideSection } from './SidePanel';
import { MumbleLink, XmppLink } from './CommLinks';
import { Pilot, Destination } from './EsiUi';
import { AccountPilot } from '../Helpers';
import { Account } from './AutocompleteInputs';

const baseUri = "/fleets";


/** Fleet Settings Buttons **/

export class BtnClose extends Component {

    closeFleet() {
        if (confirm("Are you sure you want to close this fleet?")) {
            $.ajax({
                type: 'delete',
                url: `${baseUri}/${this.props.fleetId}`
            }).done(() => {
                window.location.href = `/`
            })
        }
    }

    render() {
        return (
            <div className="col-6 py-1">
                <button className="btn btn-danger btn-block" onClick={this.closeFleet.bind(this)}>
                    Close Fleet
                    <i className="far fa-times-circle"></i>
                </button>
            </div>
        );
    }
}

export class BtnClear extends Component {
    clearWaitlist() {
        if (confirm("This will clear the waitlist for all fleets, are you sure you wish to continue?")) {
            $.ajax({
                type: 'post',
                url: `/waitlist/clear`
            }).done((message) => {
                this.props.u();
            }).fail((err) => {
                console.error(`[React/.../FleetSettings BtnClear@clearWaitlist] Error clearing the waitlist: ${err.responseText}`)
            })
        }
    }

    render() {
        return (
            <div className="col-6 py-1">
                <button className="btn btn-danger btn-block" onClick={this.clearWaitlist.bind(this)}>
                    Clear Waitlist
                    <i className="far fa-times-circle"></i>
                </button>
            </div>
        );
    }
}

export class BtnInvAll extends Component {
    render() {
        return (
            <div className="col-6 py-1">
                <button className="btn btn-warning btn-block disabled">
                    Invite All
                    <i className="fas fa-info-circle"></i>
                </button>
            </div>
        );
    }
}

export class BtnInvFaxes extends Component {
    render() {
        return (
            <div className="col-6 py-1">
                <button className="btn btn-warning btn-block disabled">
                    Invite all Faxes
                    <i className="fas fa-info-circle"></i>
                </button>
            </div>
        );
    }
}

/** Fleet Settings Options */

export class Backseat extends Component {
    getIcoId() {
        if (this.props.account && this.props.pilots)
            return AccountPilot(this.props.account.name, this.props.pilots).id

        return 0;
    }

    set() {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.props.fleetId}/backseat`,
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/FleetSettings {Backseat@set} - Error setting the fleet backseat `, err.responseText);
        });
    }

    clear() {
        $.ajax({
            type: 'delete',
            url: `${baseUri}/${this.props.fleetId}/backseat`,
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/FleetSettings {Backseat@clear} - Error clearing the backseat for this fleet`, err.responseText);
        });
    }

    render() {
        return (
            <SideSection title="Backseat">
                <div className="row sidepanel-content">
                    <div className="col-3">
                        <img className="ml-3 pr-2" src={`https://image.eveonline.com/Character/${this.getIcoId()}_64.jpg`} />
                    </div>
                    <div className="col-9">
                        <XmppLink AuthName={ (this.props.account) ? this.props.account.name : null } />

                        <div className="d-block">
                            <button className="btn btn-lg btn-dark mr-2" onClick={this.set.bind(this)}>I'm the backseat....</button>
                            <i className="fas fa-times-circle clear" onClick={this.clear.bind(this)}></i>
                        </div>
                    </div>
                </div>
            </SideSection>    
        );
    }
}

export class Boss extends Component {
    getPilot() {
        return this.props.pilot || null;
    }

    set(id) {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.props.fleetId}/boss`,
            data: { pilotId: id }
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/FleetSettings {Boss@set} - Error setting the fleet boss `, err.responseText);
        });
    }

    render() {
        let pilots;
        if (this.props.pilots) {
            pilots = this.props.pilots.map((pilot) => {
                return <a className="dropdown-item" role="presentation" onClick={this.set.bind(this, pilot.id)}>{pilot.name}</a>
            });
        }

        return (
            <SideSection title="Fleet Commander">
                <div className="row sidepanel-content">
                    <div className="col-3">
                        <img className="ml-3 pr-2" src={`https://image.eveonline.com/Character/${this.getPilot() ? this.getPilot().id : 0}_64.jpg`} />
                    </div>
                    <div className="col-9">
                        <Pilot pilot={this.getPilot()} />

                        <div className="dropdown">
                            <button class="btn btn-lg btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">I'm the Boss....</button>
                            <div class="dropdown-menu" role="menu">
                                {pilots}
                            </div>
                        </div>
                    </div>
                </div>
            </SideSection>    
        );
    }
}

export class ExitCyno_Add extends Component {
    render() {
        let pilots;
        if (this.props.pilots) {
            pilots = this.props.pilots.map((pilot) => {
                return <a className="dropdown-item" role="presentation" onClick={this.set.bind(this, pilot.id)}>{pilot.name}</a>
            });
        }

        return (
            <div className="mb-5 pb-4">
                <p className="text-danger text-center" >Incursions Squad-L would like to remind you that you MUST have exit cynos at all times.</p>

                <div className="dropdown float-right">
                    <button class="btn btn-lg btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Add fleet cyno...</button>
                    <div class="dropdown-menu" role="menu">
                        {pilots}
                    </div>
                </div>
            </div>
        )
    }
}

export class ExitCyno extends Component {
    unsetCyno(fleetId, pilotId) {
        console.log(`Unset cyno, pilot id: ${pilotId} who is in fleet ${fleetId}`);
        $.ajax({
            type: 'put',
            url: `/fleets/${fleetId}/cyno/${pilotId}`
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`[React/FleetSettings@unsetCyno] Error removing pilotId: ${pilotId} as a fleet cyno: ${err.responseText}`)
        })
    }

    render() {
        let cynos;
        if (this.props.cynos) {
            cynos = this.props.cynos.map((pilot) => {
                return (
                    <div className="row sidepanel-content">
                        <div className="col-3">
                            <img className="ml-3 pr-2" src={`https://image.eveonline.com/Character/${pilot.id}_64.jpg`} />
                        </div>
                        <div className="col-9 pl-4">
                            <Pilot pilot={pilot} />
                            <i className="fas fa-times-circle float-right mr-3" onClick={this.unsetCyno.bind(this, this.props.fleetId, pilot.id)}></i>

                            <div class="clearfix" />

                            <i className="fas fa-map-marker-alt"></i>
                            <Destination system={pilot.system} />
                        </div>
                    </div>
                )
            });
        }

        return (
            <SideSection>
                {cynos}
            </SideSection>
        );
    }
}

export class Mumble extends Component {

    get() {
        return this.props.channel || null;
    }

    getCommOptions() {
        return this.props.options || null;
    }

    set(i) {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.props.fleetId}/comms`,
            data: { commsId: i }
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/FleetSettings {Mumble@set} - Error setting the comms channel for this fleet`, err.responseText);
        });
    }



    render() {
        let commOptions;
        if (this.getCommOptions()) {
            commOptions = this.getCommOptions().map((channel) => {
                return <a className="dropdown-item" role="presentation" onClick={this.set.bind(this, channel.id)}>{channel.linkText}</a>;
            });
        }

        return (
            <SideSection title="Mumble">
                <span className="sidepanel-content">
                    <MumbleLink commChannel={this.get()} />
                </span>

                <div className="dropdown pt-2">
                    <button class="btn btn-lg btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Select Comms....</button>
                    <div class="dropdown-menu" role="menu">
                        {commOptions}
                    </div>
                </div>
            </SideSection>
        )
    }
}

export class Status extends Component {

    handleChange() {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.props.fleetId}/status`,
            data: { status: !this.props.public }
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/FleetSettings {Status@handleChange} - Error updating fleet status`, err.responseText);
        });
    }

    render() {
        return (
            <SideSection title="Fleet Status">
                <label class="switch">
                    <input type="checkbox" id="togBtn" defaultChecked={this.props.public} onChange={this.handleChange.bind(this)} />
                    <div class="slider round">
                        <span class="on">Listed</span>
                        <span class="off">Not Listed</span>
                    </div>
                </label>
            </SideSection>
        )
    }
}

export class Type extends Component {

    set(i) {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.props.fleetId}/type`,
            data: { type: i }
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/FleetSettings {Type@set} - Error setting the fleet type `, err.responseText);
        });
    }

    render() {
        let fleetTypes;
        if (this.props.options) {
            fleetTypes = this.props.options.map((type) => {
                return <a className="dropdown-item" role="presentation" onClick={this.set.bind(this, type)}>{type}</a>;
            });
        }

        return (
            <SideSection title="Fleet Type">
                <span className="sidepanel-content">
                    <i className="far fa-location"></i>
                    {this.props.type}
                </span>

                <div className="dropdown pt-2">
                    <button class="btn btn-lg btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Select Type....</button>
                    <div class="dropdown-menu" role="menu">
                        {fleetTypes}
                    </div>
                </div>
            </SideSection>
        )
    }
}