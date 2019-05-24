import React, { Component } from 'react';
import { SideSection } from './SidePanel';
import { MumbleLink } from './CommLinks';

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
    render() {
        return (
            <div className="col-6 py-1">
                <button className="btn btn-danger btn-block disabled">
                    Clear Fleet
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

export class Mumble extends Component {

    getComms() {
        return this.props.channel || null;
    }

    getCommOptions() {
        return this.props.options || null;
    }

    setComms(i) {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.props.fleetId}/comms`,
            data: { commsId: i }
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/FleetSettings {Mumble@setComms} - Error setting the comms channel for this fleet`, err.responseText);
        });
    }



    render() {
        let commOptions;
        if (this.getCommOptions()) {
            commOptions = this.getCommOptions().map((channel) => {
                return <a className="dropdown-item" role="presentation" onClick={this.setComms.bind(this, channel.id)}>{channel.linkText}</a>;
            });
        }

        return (
            <SideSection title="Mumble">
                <MumbleLink commChannel={this.getComms()} />

                <div className="dropdown pt-2">
                    <button class="btn btn-dark mx-auto dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Select Comms....</button>
                    <div class="dropdown-menu" role="menu">
                        {commOptions}
                    </div>
                </div>
            </SideSection>
        )
    }
}

export class Status extends Component {

    handleCheckboxChange() {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.props.fleetId}/status`,
            data: { status: !this.props.public }
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/FleetSettings {Status@handleCheckboxChange} - Error updating fleet status`, err.responseText);
        });
    }

    render() {
        return (
            <SideSection title="Fleet Status">
                <label class="switch">
                    <input type="checkbox" id="togBtn" defaultChecked={this.props.public} onChange={this.handleCheckboxChange.bind(this)} />
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

    setType(i) {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.props.fleetId}/type`,
            data: { type: i }
        }).done(() => {
            this.props.u();
        }).fail((err) => {
            console.error(`React/FleetSettings {Type@setType} - Error setting the type for this fleet`, err.responseText);
        });
    }

    render() {
        let fleetTypes;
        if (this.props.options) {
            fleetTypes = this.props.options.map((type) => {
                return <a className="dropdown-item" role="presentation" onClick={this.setType.bind(this, type)}>{type}</a>;
            });
        }

        return (
            <SideSection title="Fleet Type">
                <span className="mumble">
                    <i className="far fa-location pr-4"></i>
                    {this.props.type}
                </span>

                <div className="dropdown pt-2">
                    <button class="btn btn-dark mx-auto dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Select Type....</button>
                    <div class="dropdown-menu" role="menu">
                        {fleetTypes}
                    </div>
                </div>
            </SideSection>
        )
    }
}