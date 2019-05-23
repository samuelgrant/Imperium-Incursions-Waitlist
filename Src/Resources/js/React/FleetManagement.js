import React, { Component } from 'react';
import { render } from 'react-dom';
import Alert from './Components/Alert';
import { SidePanel, SideSection, SidePanelButton } from './Components/SidePanel';
import { MumbleLink, XmppLink } from './Components/CommLinks';
import { Pilot } from './Components/EsiUi';

const baseUri = "/fleets";

export default class Index extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fcOptions: null,
            fleetId: 1
        }
    }

    componentDidMount() {
        this.getFcSettings();

        this.setState({
            fleetId: $("#fleetManagement").data("fleetid")
        }, () => this.getData());
    }

    closeFleet() {
        if (confirm("Are you sure you want to close this fleet?")) {
            $.ajax({
                type: 'delete',
                url: `${baseUri}/${this.state.fleetId}`
            }).done(() => {
                window.location.href = `/`
            })
        }
    }

    isPublic() {
        return (this.state.fleet) ? this.state.fleet.isPublic : null;
    }

    handleCheckboxChange() {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.state.fleetId}/status`,
            data: { status: !this.isPublic() }
        }).done(() => {
            this.getData();
        }).fail((err) => {
            console.error(`React/FleetManagement {FleetManagement@handleCheckboxChange} - Error updating fleet status`, err.responseText);
        });
    }

    getData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/${this.state.fleetId}/data`,
        }).done((result) => {
            this.setState({ fleet: result });
        }).fail((err) => {
            console.error(`React/FleetManagement {FleetManagement@getData} - Error getting fleet information`, err.responseText);
        });
    }

    getFcSettings() {
        $.ajax({
            type: 'get',
            url: `/waitlist/fcsettings`,
            async: false
        }).done((settings) => {
            this.setState({ fcOptions: settings });
        }).fail((err) => {
            console.error(`React/FleetManagement {Index@getFcSettings} - Error getting the FC settings`, err.responseText);
        })
    }

    getFleetSettings() {
        return this.state.fleet || null;
    }

    setComms(i) {
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.state.fleetId}/comms`,
            data: { commsId: i }
        }).done(() => {
            this.getData();
        }).fail((err) => {
            console.error(`React/FleetManagement {Index@setComms} - Error setting the comms channel for this fleet`, err.responseText);
        });
    }

    setType(i) {
        console.log(i);
        $.ajax({
            type: 'put',
            url: `${baseUri}/${this.state.fleetId}/type`,
            data: { type: i }
        }).done(() => {
            this.getData();
        }).fail((err) => {
            console.error(`React/FleetManagement {Index@setComms} - Error setting the comms channel for this fleet`, err.responseText);
        });
    }

    render() {
        let fleetPrivate;
        if (!this.isPublic()) {
            fleetPrivate = (
                <Alert type="danger">
                    <span className="font-weight-bold">Fleet Not Listed: </span>
                    If no fleets are listed, the waitlist will show as offline.
                </Alert>
            );
        }

        let commOptions;
        if (this.state.fcOptions) {
            commOptions = this.state.fcOptions.comms.map((channel) => {
                return <a className="dropdown-item" role="presentation" onClick={this.setComms.bind(this, channel.id)}>{channel.linkText}</a>;
            });
        }

        let fleetTypes;
        if (this.state.fcOptions) {
            fleetTypes = this.state.fcOptions.fleetTypes.map((type) => {
                return <a className="dropdown-item" role="presentation" onClick={this.setType.bind(this, type)}>{type}</a>;
            });
        }

        let boss_myPilots;
        if (this.state.fcOptions) {
            boss_myPilots = this.state.fcOptions.pilots.map((pilot) => {
                return <a className="dropdown-item" role="presentation">{pilot.name}</a>
            });
        }

        let backSeat_myPilots;
        if (this.state.fcOptions) {
            backSeat_myPilots = this.state.fcOptions.pilots.map((pilot) => {
                return <a className="dropdown-item" role="presentation">{pilot.name}</a>
            });
        }
        

        return (
            <div className="container">
                {fleetPrivate}
                <SidePanelButton id="fleetSettings" title="Fleet Settings" />
                <SidePanelButton id="fleetCynos" title="Fleet Cynos" />

                <SidePanel id="fleetSettings" title="Fleet Settings">
                    <div className="row">
                        <SideSection title="Fleet Commander">
                            <div className="row mumble">
                                <div className="col-3">
                                    <img className="ml-3 pr-2" src={`https://image.eveonline.com/Character/${96304094}_64.jpg`} />
                                </div> 
                                <div className="col-9">
                                    <Pilot pilot={(this.state.fleet) ? this.state.fleet.bossPilot : null} />
                                

                                    <div className="dropdown">
                                        <button class="btn btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">I'm the Boss....</button>
                                        <div class="dropdown-menu" role="menu">
                                            {boss_myPilots}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </SideSection>

                        <SideSection title="Backseat">
                            <div className="row mumble">
                                <div className="col-3">
                                    <img className="ml-3 pr-2" src={`https://image.eveonline.com/Character/${96304094}_64.jpg`} />
                                </div>
                                <div className="col-9">
                                    <XmppLink AuthName={(this.state.fleet) ? this.state.fleet.bossPilot.name : null} />
                                    <i className="fas fa-times-circle clear"></i>


                                    <div className="dropdown">
                                        <button class="btn btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">I'm the Backseat....</button>
                                        <div class="dropdown-menu" role="menu">
                                            {backSeat_myPilots}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </SideSection>

                        <SideSection title="Mumble">
                            <MumbleLink commChannel={(this.getFleetSettings()) ? this.getFleetSettings().commChannel : null} />
                            
                            <div className="dropdown pt-2">
                                <button class="btn btn-dark mx-auto dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Select Comms....</button>
                                <div class="dropdown-menu" role="menu">
                                    {commOptions}
                                </div>
                            </div>
                        </SideSection>

                        <SideSection title="Fleet Type">
                            <span className="mumble">
                                <i className="far fa-location pr-4"></i>
                                {(this.getFleetSettings()) ? this.getFleetSettings().type : ""}
                            </span>

                            <div className="dropdown pt-2">
                                <button class="btn btn-dark mx-auto dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">Select Type....</button>
                                <div class="dropdown-menu" role="menu">
                                    {fleetTypes}
                                </div>
                            </div>
                        </SideSection>

                        <SideSection title="Fleet Status">
                            <label class="switch">
                                <input type="checkbox" id="togBtn" defaultChecked={this.isPublic()} onChange={this.handleCheckboxChange.bind(this)}/>
                                    <div class="slider round">
                                        <span class="on">Listed</span>
                                        <span class="off">Not Listed</span>
                                    </div>
                            </label>
                        </SideSection>
                    </div>

                    <hr />

                    <div className="row">
                        <div className="col-6 py-1">
                            <button className="btn btn-danger btn-block" onClick={this.closeFleet.bind(this)}>
                                Close Fleet
                                <i className="far fa-times-circle"></i>
                            </button>
                        </div>
                        <div className="col-6 py-1">
                            <button className="btn btn-danger btn-block disabled">
                                Clear Waitlist
                                <i className="far fa-times-circle"></i>
                            </button>
                        </div>

                        <div className="col-6 py-1">
                            <button className="btn btn-warning btn-block disabled">
                                Invite All
                                <i className="fas fa-info-circle"></i>
                            </button>
                        </div>
                        <div className="col-6 py-1">
                            <button className="btn btn-warning btn-block disabled">
                                Invite all Faxes
                                <i className="fas fa-info-circle"></i>
                            </button>
                        </div>
                    </div>
                </SidePanel>
                
                    
                <SidePanel id="fleetCynos" title="Fleet Cynos">

                </SidePanel>
            </div>
        )
    }
}



if (document.getElementById('fleetManagement'))
    render(<Index />, document.getElementById('fleetManagement'));